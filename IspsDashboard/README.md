# Sûreté ISPS — Côte d'Ivoire Terminal

Application web de sûreté portuaire conforme au **Code ISPS** (International Ship and Port Facility Security Code), développée pour le terminal à conteneurs de Treichville PC6, Abidjan.

> Référence cahier des charges : **CDC-ISPS-2026-001**

---

## 1. Stack technique

| Couche | Technologie |
|--------|-------------|
| Framework | ASP.NET Core **MVC (.NET 9)** — projet unique, architecture en couches |
| Base de données | **SQL Server 2022** + Entity Framework Core 9 (code-first, migrations) |
| Authentification | ASP.NET Core **Identity** (cookies), 3 rôles |
| Temps réel | **SignalR** (mise à jour du tableau de bord sans rechargement) |
| Tâches planifiées | **Quartz.NET** (alertes exercices J-30 / J-7, 08:00 Abidjan) |
| Emails | **MailKit** (SMTP configurable en BDD, mot de passe chiffré) |
| PDF | **QuestPDF** (rapport mensuel sûreté) |
| Front | Razor + **Tailwind CSS** (build compilé, voir §2 bis) + **Chart.js** |
| Sécurité HTTP | CSP par nonce (aucun `unsafe-inline`), X-Frame-Options, X-Content-Type-Options, Referrer-Policy |
| Logs | **Serilog** (console + fichier rotatif quotidien `logs/`) |
| Mobile | **PWA** installable (manifest + service worker) |
| Tests | **xUnit** (85 tests) + EF Core InMemory |

Architecture en couches dans un seul projet :
```
IspsDashboard/
├── Controllers/        # 24 contrôleurs MVC
├── Models/
│   ├── Entities/       # entités EF Core
│   ├── ViewModels/     # modèles de vue
│   └── Enums/          # énumérations métier
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Migrations/
│   └── Seed/           # DataSeeder + SampleDataSeeder
├── Services/
│   ├── Interfaces/     # contrats
│   └── Implementations/
├── Hubs/               # SignalR
├── Jobs/               # Quartz
├── Views/              # vues Razor par module
└── wwwroot/            # statique, uploads, PWA, icônes
```

---

## 2. Prérequis

- **.NET SDK 9.0+** (poste de build)
- **SQL Server 2022** (ou Express) accessible
- **Production sur IIS** : Windows Server + IIS + le **.NET 9 Hosting Bundle** (module ASP.NET Core)
- (Optionnel) un serveur SMTP pour les alertes email
- **Node.js 20+** — uniquement pour recompiler le CSS Tailwind après avoir ajouté/modifié des classes dans les vues (voir §2 bis). Pas nécessaire pour lancer l'appli telle quelle : le CSS compilé (`wwwroot/css/tailwind.css`) est déjà versionné.

### 2 bis. CSS Tailwind — build compilé, pas de CDN

L'appli n'utilise **plus** le CDN "Play" de Tailwind (`cdn.tailwindcss.com`) — il est incompatible avec une politique CSP stricte (`style-src` sans `unsafe-inline`) et déconseillé par Tailwind lui-même en production. À la place, `wwwroot/css/tailwind.css` est un fichier **compilé et versionné**, généré depuis `wwwroot/css/tailwind-src.css` en scannant les classes utilisées dans `Views/**/*.cshtml` (`tailwind.config.js`).

**Après toute modification de classes Tailwind dans une vue**, recompiler avant de committer :
```bash
cd IspsDashboard
npm install          # une seule fois
npm run build:css    # régénère wwwroot/css/tailwind.css
```
La CI (`.github/workflows/ci.yml`) échoue automatiquement si `tailwind.css` n'a pas été régénéré après un changement de classes — c'est le filet de sécurité si ce réflexe est oublié.

---

## 3. Configuration

### Chaîne de connexion — `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=IspsDashboard;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```
> `appsettings.json` / `appsettings.Production.json` ne contiennent **aucun identifiant réel** : la valeur ci-dessus est un placeholder Windows Auth/localhost inoffensif. Le vrai serveur/compte SQL doit être fourni hors dépôt :
> - **En développement** : `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;User Id=...;Password=...;..."` (stocké hors du repo, chargé automatiquement en environnement `Development`).
> - **En production (IIS)** : variable d'environnement `ConnectionStrings__DefaultConnection`, définie au niveau du **pool d'applications IIS** (IIS Manager → Application Pools → *Set Application Pool Defaults* / *Advanced Settings* → `Environment Variables`, ou via `appcmd.exe set config -section:system.applicationHost/applicationPools ...`). Sans cette variable, l'app tente `localhost` avec authentification Windows et échoue explicitement au démarrage plutôt que d'utiliser un identifiant par défaut connu.

### Compte administrateur & amorçage — section `Seed`
```json
"Seed": {
  "AdminEmail": "admin@cit.ci",
  "AdminPassword": "Changez-Moi-2026!",
  "AdminFullName": "Responsable Adjoint Sûreté",
  "ReferenceData": false,
  "SampleData": false
}
```
> `Seed:AdminPassword` est vide par défaut dans les deux fichiers — il **doit** être fourni via variable d'environnement (`Seed__AdminPassword`) ou User Secrets en dev (`dotnet user-secrets set "Seed:AdminPassword" "..."`). Il doit respecter la politique (≥ 8 caractères, 1 majuscule, 1 chiffre, 1 caractère spécial) — sinon le compte admin n'est pas créé et le démarrage échoue avec une erreur explicite. C'est volontaire : aucun mot de passe par défaut connu ne doit pouvoir arriver en production.

Deux niveaux d'amorçage, indépendants :

| Flag | Contenu | Prod (`appsettings.json`) | Dev (`appsettings.Development.json`) |
|------|---------|:---:|:---:|
| `ReferenceData` | Données de référence : paramètres tableau de bord, 23 KPI, 12 agents, 12 caméras, 8 checkpoints, 6 contacts, 5 zones… | **false** | true |
| `SampleData` | Données de démonstration : laissez-passer, mouvements, visiteurs, incidents, documents | **false** | true |

➡️ **En production, la base démarre vide : uniquement les 3 rôles + le compte admin.** Tout le reste se crée ensuite via l'interface (Administration + modules). Mettez un flag à `true` si vous souhaitez pré-remplir.

### HTTP / HTTPS — section `Hosting`
```json
"Hosting": { "UseHttps": false }
```
> `false` (défaut) : l'application sert en **HTTP** (pas de redirection HTTPS ni HSTS) — adapté à un hébergement interne ou derrière IIS/reverse proxy qui gère le protocole. Mettre `true` pour forcer HTTPS + HSTS au niveau applicatif.

### SMTP (alertes email)
Configurable **directement depuis l'interface** : Administration → onglet **📧 Notifications**. Le mot de passe est chiffré (DataProtection) avant stockage. Un bouton *Envoyer un email de test* permet de valider.

---

## 4. Première exécution

```bash
# 1. Restaurer et compiler
dotnet build

# 2. Appliquer les migrations (création automatique de la base au démarrage,
#    ou explicitement) :
dotnet ef database update

# 3. Lancer
dotnet run
```

L'application démarre sur l'URL indiquée dans la console (ex. `http://localhost:5090`).
En développement (`ReferenceData=true`), les données de référence sont pré-remplies. En production, seuls les rôles + le compte admin sont créés.

> ⚠️ `dotnet run` force l'environnement **Development** (via `launchSettings.json`). Pour tester le comportement « production » localement : `dotnet run --no-launch-profile` avec `ASPNETCORE_ENVIRONMENT=Production`.

**Connexion** : `admin@cit.ci` / mot de passe défini dans `Seed:AdminPassword`.

### Vérifier la santé
- `GET /health` → état de l'application + connexion SQL Server.

---

## 5. Rôles & droits

| Rôle | Droits |
|------|--------|
| **Administrateur** | Tout : configuration, utilisateurs, journal d'audit, données |
| **Éditeur** | Saisie et modification des données opérationnelles |
| **Lecteur** | Consultation seule |

Création des comptes : Administration → **👤 Utilisateurs**.

---

## 6. Modules fonctionnels

**Pilotage** — Tableau de bord (KPI, jauges, radar, agents), widget « Mon poste » consolidé, Statistiques 12 mois.

**Contrôle d'accès** — Laissez-passer (journaliers / trimestriels / annuels, personnes & véhicules), Journal des véhicules & camions (plaques, conteneurs, scellés), Visiteurs (check-in/out).

**Opérations** — Incidents, Patrouilles (checkpoints QR + géolocalisation), Navires & Déclarations de Sûreté (DoS), Briefings de prise de poste, Niveau MARSEC (1/2/3 avec checklists).

**Conformité** — Non-conformités (CAPA), Audits internes (checklist ISPS Part B), REX d'exercices, Rapports PDF, Journal d'audit.

**Ressources** — Agents & Habilitations (alertes d'expiration), Caméras CCTV (disponibilité, maintenance), Annuaire externe, Documents PFSP versionnés.

---

## 7. Sauvegarde & exploitation

- **Base de données** : planifier des sauvegardes SQL Server régulières (la base contient tout l'historique sûreté — donnée sensible).
- **Uploads** : le dossier `wwwroot/uploads/` (pièces jointes incidents, photos agents, documents) doit être inclus dans les sauvegardes.
- **Clés DataProtection** : `App_Data/keys/` — à sauvegarder et **conserver** (sinon les secrets chiffrés, dont le mot de passe SMTP, deviennent illisibles).
- **Logs** : `logs/isps-*.log`, rotation quotidienne, rétention 30 jours.

---

## 8. Déploiement sur IIS (Windows Server)

> IIS n'utilise **pas** `launchSettings.json` → l'environnement est **Production** par défaut → `appsettings.Development.json` n'est pas chargé → base vide (rôles + admin) et HTTP, conformément à `appsettings.json` / `appsettings.Production.json`.

### Étapes

1. **Serveur** : activer le rôle **IIS** + installer le **.NET 9 Hosting Bundle**
   (https://dotnet.microsoft.com/download/dotnet/9.0 → « ASP.NET Core Runtime — Hosting Bundle »).
   Puis `iisreset` pour charger le module ASP.NET Core.

2. **Publier** (poste de build) :
   ```bash
   dotnet publish IspsDashboard/IspsDashboard.csproj -c Release -o C:\inetpub\ispsdashboard
   ```
   Le `web.config` (handler ASP.NET Core) est généré automatiquement.

3. **Site IIS** : nouveau site pointant sur `C:\inetpub\ispsdashboard`, **liaison HTTP** sur le port voulu (80 / 8080).

4. **Pool d'applications** : .NET CLR = **« Aucun code managé »** (hébergement in-process via ANCM).

5. **Permissions** — donner à l'identité du pool (`IIS AppPool\<NomDuSite>`) un accès **écriture** sur :
   - `App_Data\keys` — clés DataProtection (sinon le mot de passe SMTP chiffré devient illisible au redémarrage)
   - `logs\` — journaux Serilog
   - `wwwroot\uploads\` — pièces jointes, photos, documents

6. **Base de données** : autoriser l'identité du pool sur SQL Server, **ou** renseigner un compte SQL dans `ConnectionStrings:DefaultConnection` de `appsettings.Production.json`.

7. **Avant le 1er démarrage** : définir `ConnectionStrings__DefaultConnection` et `Seed__AdminPassword` en variables d'environnement du pool d'applications IIS (jamais dans `appsettings.Production.json`, qui reste sans secret réel).

8. **Vérifier** : ouvrir `http://<serveur>:<port>/health` → doit répondre *Healthy*. Se connecter avec `admin@cit.ci`.

### HTTPS plus tard (optionnel)
Le plus simple sur IIS : ajouter une **liaison HTTPS au site IIS** (certificat géré par IIS). Aucune modification applicative requise (laisser `Hosting:UseHttps=false`).

### Checklist de durcissement

| Point | Action |
|-------|--------|
| Mot de passe admin | Changé (`Seed:AdminPassword`) — politique respectée |
| `ReferenceData` / `SampleData` | `false` (base vide sauf admin) |
| Secrets | Chaîne de connexion + `Seed:AdminPassword` définis via variables d'environnement du pool IIS (jamais en clair dans les fichiers `appsettings*.json`) |
| Permissions dossiers | `App_Data\keys`, `logs`, `wwwroot\uploads` en écriture pour le pool |
| Sauvegardes | BDD + `wwwroot\uploads` + `App_Data\keys` |
| SMTP | Configurer + tester (Administration → Notifications) |
| Intégration Teams | Épingler l'URL comme onglet web dans un canal |

---

## 9. Tests

```bash
cd IspsDashboard.Tests
dotnet test
```
85 tests unitaires couvrant la logique métier critique (codes couleur exercices, scores radar, expiration habilitations et laissez-passer, références séquentielles, score de conformité audit, chiffrement des secrets, etc.), ainsi que des tests de régression dédiés : assignation de masse (10 services) et concurrence optimiste (RowVersion).

---

## 10. Conformité ISPS — note honnête

L'application **couvre les domaines opérationnels** d'une installation portuaire certifiée ISPS (contrôle d'accès, surveillance, gestion des niveaux de sûreté, DoS navires, incidents, exercices/REX, audits, non-conformités, documentation, traçabilité par journal d'audit).

Elle constitue un **outil de pilotage et de traçabilité**. Une conformité réglementaire effective dépend aussi d'éléments **hors logiciel** qui relèvent de l'exploitant :

- le **PFSP réel** approuvé par l'autorité compétente (le module Documents en gère la version, pas le contenu) ;
- les **procédures internes** et la formation effective du personnel ;
- la **configuration locale** (contacts réels des autorités, seuils, périmètre des caméras et checkpoints) ;
- l'**hébergement sécurisé** et les sauvegardes ;
- la **validation par l'Agent de Sûreté de l'Installation Portuaire (PFSO/RSO)** et, le cas échéant, par l'autorité maritime.

L'outil facilite et documente la mise en œuvre du Code ISPS ; il ne se substitue pas à la responsabilité réglementaire de l'exploitant.

---

*CDC-ISPS-2026-001 · Côte d'Ivoire Terminal · Document technique — diffusion restreinte.*

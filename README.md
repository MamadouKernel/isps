# ISPS — Sûreté portuaire

Tableau de bord de sûreté portuaire conforme au **Code ISPS**, pour le terminal à conteneurs Côte d'Ivoire Terminal (Treichville PC6, Abidjan).

- **Application** : [`IspsDashboard/`](IspsDashboard/) — voir [`IspsDashboard/README.md`](IspsDashboard/README.md) pour la stack technique, la configuration, le déploiement et l'exploitation.
- **Tests** : [`IspsDashboard.Tests/`](IspsDashboard.Tests/) — 73 tests xUnit (`dotnet test`).

## Démarrage rapide

```bash
cd IspsDashboard
dotnet user-secrets set "Seed:AdminPassword" "VotreMotDePasse-2026!"
dotnet run
```

Aucun secret réel n'est versionné : le mot de passe administrateur et la chaîne de connexion doivent être fournis via `dotnet user-secrets` (développement) ou des variables d'environnement (production) — voir le README de l'application pour le détail.

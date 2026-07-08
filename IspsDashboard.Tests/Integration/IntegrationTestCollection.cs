namespace IspsDashboard.Tests.Integration;

/// <summary>
/// Une seule instance de CustomWebApplicationFactory (donc une seule exécution des instructions
/// de haut niveau de Program.cs) partagée par toutes les classes de tests d'intégration : en
/// créer une par classe fait planter Serilog ("The logger is already frozen") au deuxième
/// hôte démarré dans le même process, Log.Logger étant un champ statique global.
/// </summary>
[CollectionDefinition("Integration")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}

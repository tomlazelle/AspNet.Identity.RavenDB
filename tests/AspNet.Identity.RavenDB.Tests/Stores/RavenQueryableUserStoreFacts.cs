using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Raven.Client;
using System.Threading.Tasks;
using NUnit.Framework;


namespace AspNet.Identity.RavenDB.Tests.Stores
{
    [TestFixture]
    public class RavenQueryableUserStoreTests : TestBase
    {
        [Test]
        public async Task RavenUserStore_Users_Should_Expose_IQueryable_Over_IRavenQueryable()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            {
                const string userName = "Tugberk";
                const string userNameToSearch = "TugberkUgurlu";

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    RavenUser user = new RavenUser(userName);
                    RavenUser userToSearch = new RavenUser(userNameToSearch);
                    await ses.StoreAsync(user);
                    await ses.StoreAsync(userToSearch);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    // Act
                    ses.Advanced.UseOptimisticConcurrency = true;
                    RavenUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                    RavenUser retrievedUser = await userStore.Users.FirstOrDefaultAsync(user => user.UserName == userNameToSearch);

                    // Assert
                    Assert.NotNull(retrievedUser);
                    Assert.AreEqual(userNameToSearch, retrievedUser.UserName);
                }
            }
        }
    }
}
using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    [TestFixture]
    public class RavenUserTwoTestorStoreTests : TestBase
    {
        [Test]
        public async Task GetTwoTestorEnabledAsync_Should_Get_User_IsTwoTestorEnabled_Value()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            {
                const string userName = "Tugberk";
                const string userId = "RavenUsers/Tugberk";

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    RavenUser user = new RavenUser(userName);
                    user.EnableTwoFactorAuthentication();
                    await ses.StoreAsync(user);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    // Act
                    ses.Advanced.UseOptimisticConcurrency = true;
                    RavenUser user = await ses.LoadAsync<RavenUser>(userId);
                    IUserTwoFactorStore<RavenUser, string> userTwoTestorStore = new RavenUserStore<RavenUser>(ses);
                    bool isTwoTestorEnabled = await userTwoTestorStore.GetTwoFactorEnabledAsync(user);

                    // Assert
                    Assert.True(isTwoTestorEnabled);
                }
            }
        }

        [Test]
        public async Task SetTwoTestorEnabledAsync_Should_Set_IsTwoTestorEnabled_Value()
        {
            using (IDocumentStore store = CreateEmbeddableStore())
            {
                const string userName = "Tugberk";
                const string userId = "RavenUsers/Tugberk";

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    RavenUser user = new RavenUser(userName);
                    user.EnableTwoFactorAuthentication();
                    await ses.StoreAsync(user);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    // Act
                    ses.Advanced.UseOptimisticConcurrency = true;
                    RavenUser user = await ses.LoadAsync<RavenUser>(userId);
                    IUserTwoFactorStore<RavenUser, string> userTwoTestorStore = new RavenUserStore<RavenUser>(ses);
                    await userTwoTestorStore.SetTwoFactorEnabledAsync(user, enabled: true);

                    // Assert
                    Assert.True(user.IsTwoFactorEnabled);
                }
            }
        }
    }
}

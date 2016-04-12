using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AspNet.Identity.RavenDB.Tests.Stores
{
    [TestFixture]
    public class RavenUserClaimStoreTests : TestBase
    {
        [Test]
        public async Task GetUserClaims_Should_Retrieve_Correct_Claims_For_User()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            {
                RavenUser user = new RavenUser(userName);
                IEnumerable<RavenUserClaim> claims = new List<RavenUserClaim>
                {
                    new RavenUserClaim("Scope", "Read"),
                    new RavenUserClaim("Scope", "Write")
                };

                foreach (RavenUserClaim claim in claims)
                {
                    user.AddClaim(claim);
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    IUserClaimStore<RavenUser> userClaimStore = new RavenUserStore<RavenUser>(ses);

                    await ses.StoreAsync(user);
                    await ses.SaveChangesAsync();
                }

                using (IAsyncDocumentSession ses = store.OpenAsyncSession())
                {
                    ses.Advanced.UseOptimisticConcurrency = true;
                    IUserClaimStore<RavenUser> userClaimStore = new RavenUserStore<RavenUser>(ses);
                    IEnumerable<Claim> retrievedClaims = await userClaimStore.GetClaimsAsync(user);

                    Assert.AreEqual(2, claims.Count());
                    Assert.AreEqual("Read", claims.ElementAt(0).ClaimValue);
                    Assert.AreEqual("Write", claims.ElementAt(1).ClaimValue);
                }
            }
        }

        [Test]
        public async Task GetUserClaims_Should_Not_Return_Null_If_User_Has_No_Claims()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                ses.Advanced.UseOptimisticConcurrency = true;
                IUserClaimStore<RavenUser> userClaimStore = new RavenUserStore<RavenUser>(ses, false);
                RavenUser user = new RavenUser(userName);

                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                IEnumerable<Claim> retrievedClaims = await userClaimStore.GetClaimsAsync(user);

                // Assert
                Assert.AreEqual(0, retrievedClaims.Count());
            }
        }

        [Test]
        public async Task AddClaimAsync_Should_Add_The_Claim_Into_The_User_Claims_Collection()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = base.CreateEmbeddableStore())
            using(IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                ses.Advanced.UseOptimisticConcurrency = true;
                IUserClaimStore<RavenUser> userClaimStore = new RavenUserStore<RavenUser>(ses, false);
                RavenUser user = new RavenUser(userName);

                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                Claim claimToAdd = new Claim(ClaimTypes.Role, "Customer");
                await userClaimStore.AddClaimAsync(user, claimToAdd);

                Assert.AreEqual(1, user.Claims.Count());
                Assert.AreEqual(claimToAdd.Value, user.Claims.FirstOrDefault().ClaimValue);
                Assert.AreEqual(claimToAdd.Type, user.Claims.FirstOrDefault().ClaimType);
            }
        }

        [Test]
        public async Task RemoveClaimAsync_Should_Remove_Claim_From_The_User_Claims_Collection()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using(IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                // Arrange
                ses.Advanced.UseOptimisticConcurrency = true;
                IUserClaimStore<RavenUser> userClaimStore = new RavenUserStore<RavenUser>(ses, false);
                RavenUser user = new RavenUser(userName);

                Claim claimToAddAndRemove = new Claim(ClaimTypes.Role, "Customer");
                user.AddClaim(new RavenUserClaim(claimToAddAndRemove));

                await ses.StoreAsync(user);
                await ses.SaveChangesAsync();

                // Act
                await userClaimStore.RemoveClaimAsync(user, claimToAddAndRemove);

                // Assert
                Assert.AreEqual(0, user.Claims.Count());
            }
        }
    }
}
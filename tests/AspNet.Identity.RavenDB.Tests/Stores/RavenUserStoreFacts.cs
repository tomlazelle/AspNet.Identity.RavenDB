﻿using AspNet.Identity.RavenDB.Entities;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;


namespace AspNet.Identity.RavenDB.Tests.Stores
{
    [TestFixture]
    public class RavenUserStoreTests : TestBase
    {
        [Test]
        public async Task Should_Create_User()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                ses.Advanced.UseOptimisticConcurrency = true;
                IUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                await userStore.CreateAsync(new RavenUser(userName));

                IUser user = (await ses.Query<RavenUser>()
                    .Where(usr => usr.UserName == userName)
                    .Take(1)
                    .ToListAsync()
                    .ConfigureAwait(false)).FirstOrDefault();

                Assert.NotNull(user);
            }
        }

        [Test]
        public async Task CreateAsync_Should_Create_User_By_Putting_The_UserName_As_The_Key()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                ses.Advanced.UseOptimisticConcurrency = true;
                IUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                await userStore.CreateAsync(new RavenUser(userName));

                IUser user = (await ses.Query<RavenUser>()
                    .Where(usr => usr.UserName == userName)
                    .Take(1)
                    .ToListAsync()
                    .ConfigureAwait(false)).FirstOrDefault();

                Assert.NotNull(user);
                Assert.AreEqual(string.Format(Constants.RavenUserKeyTemplate, userName), user.Id);
            }
        }

        [Test]
        public async Task Should_Retrieve_User_By_UserName()
        {
            string userName = "Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                ses.Advanced.UseOptimisticConcurrency = true;
                IUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser(userName));
                await ses.SaveChangesAsync();

                IUser user = await userStore.FindByNameAsync(userName);

                Assert.NotNull(user);
                Assert.AreEqual(userName, user.UserName);
            }
        }

        [Test]
        public async Task Should_Return_Null_For_Non_Existing_User_By_UserName()
        {
            string userName = "Tugberk";
            string nonExistingUserName = "Tugberk2";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                ses.Advanced.UseOptimisticConcurrency = true;
                IUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser(userName));
                await ses.SaveChangesAsync();

                IUser user = await userStore.FindByNameAsync(nonExistingUserName);

                Assert.Null(user);
            }
        }

        [Test]
        public async Task Should_Retrieve_User_By_UserId()
        {
            string userName = "Tugberk";
            string userId = "RavenUsers/Tugberk";

            using (IDocumentStore store = CreateEmbeddableStore())
            using (IAsyncDocumentSession ses = store.OpenAsyncSession())
            {
                ses.Advanced.UseOptimisticConcurrency = true;
                IUserStore<RavenUser> userStore = new RavenUserStore<RavenUser>(ses);
                await ses.StoreAsync(new RavenUser(userName));
                await ses.SaveChangesAsync();

                IUser user = await userStore.FindByIdAsync(userId);

                Assert.NotNull(user);
                Assert.AreEqual(userName, user.UserName);
            }
        }
    }
}
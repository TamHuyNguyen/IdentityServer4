﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.UnitTests.Validation.Secrets
{
    public class ClientSecretValidation
    {
        const string Category = "Secrets - Client Secret Validator";

        [Fact]
        [Trait("Category", Category)]
        public async Task confidential_client_with_correct_secret_should_be_able_to_request_token()
        {
            var validator = Factory.CreateClientSecretValidator();

            var context = new DefaultHttpContext();
            var body = "client_id=roclient&client_secret=secret";

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var result = await validator.ValidateAsync(context);

            result.IsError.Should().BeFalse();
            result.Client.ClientId.Should().Be("roclient");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task confidential_client_with_incorrect_secret_should_not_be_able_to_request_token()
        {
            var validator = Factory.CreateClientSecretValidator();

            var context = new DefaultHttpContext();
            var body = "client_id=roclient&client_secret=invalid";

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var result = await validator.ValidateAsync(context);

            result.IsError.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task public_client_without_secret_should_be_able_to_request_token()
        {
            var validator = Factory.CreateClientSecretValidator();

            var context = new DefaultHttpContext();
            var body = "client_id=roclient.public";

            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentType = "application/x-www-form-urlencoded";

            var result = await validator.ValidateAsync(context);

            result.IsError.Should().BeFalse();
            result.Client.ClientId.Should().Be("roclient.public");
            result.Client.RequireClientSecret.Should().BeFalse();
        }

    }
}
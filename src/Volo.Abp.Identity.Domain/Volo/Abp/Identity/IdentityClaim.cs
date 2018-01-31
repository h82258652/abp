﻿using System;
using System.Security.Claims;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Identity
{
    public abstract class IdentityClaim : Entity<Guid>
    {
        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public virtual string ClaimType { get; protected set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public virtual string ClaimValue { get; protected set; }

        protected IdentityClaim()
        {

        }

        protected internal IdentityClaim(Guid id, [NotNull] Claim claim)
            : this(id, claim.Type, claim.Value)
        {

        }

        protected internal IdentityClaim(Guid id, [NotNull] string claimType, string claimValue)
        {
            Check.NotNull(claimType, nameof(claimType));

            Id = id;
            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        /// <summary>
        /// Creates a Claim instance from this entity.
        /// </summary>
        /// <returns></returns>
        public virtual Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }

        public virtual void SetClaim([NotNull] Claim claim)
        {
            Check.NotNull(claim, nameof(claim));

            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Encryption
{
    public class EncryptionMigrator : IEncryptionMigrator
    {
        /// <inheritdoc />
        public void EncryptDatabase(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var entityTypes = context.Model.GetEntityTypes().Where(x => !x.IsQueryType).Select(x => x.ClrType);
            
            var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set));

            if (setMethod == null)
            {
                throw new InvalidOperationException("Context object did not contain Set method!");
            }

            foreach (var entityType in entityTypes)
            {
                var setRef = setMethod.MakeGenericMethod(entityType);
                var set = (dynamic) setRef.Invoke(context, new object[0]);

                foreach (var entity in set)
                {
                    context.Entry(entity).State = EntityState.Modified;
                }
            }

            context.SaveChanges();
        }
    }
}
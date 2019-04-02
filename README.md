# EntityFrameworkCore.Encryption

[![Build Status](https://travis-ci.org/vbrosch/EntityFrameworkCore.Encryption.svg?branch=master)](https://travis-ci.org/vbrosch/EntityFrameworkCore.Encryption)
[![NuGet version](https://badge.fury.io/nu/EntityFrameworkCore.Encryption.svg)](https://badge.fury.io/nu/EntityFrameworkCore.Encryption)

An encryption layer for EntityFrameworkCore. It can be used to encrypt entities in your database which may be required to
be GDPR compliant or due to privacy concerns. It's based on `AES-256` and using the internal AES implementation of .NET Core.

:rotating_light: **Disclaimer/Warning:** This library is still in active development, use it at your own risk. I've only tested it with Npgsql (PostgreSQL)
and sqlite. Other providers may cause unknown issues. I'm not already using it in production myself.

If used in production, it goes without saying that you should create a backup of your unencrypted database **before**
using it.

# Setup / Usage
You can install the library via the NuGet package `EntityFrameworkCore.Encryption` easily. However, if you want to use the KeyGenerator, it is required to clone the source. 

```
    $ git clone https://github.com/vbrosch/EntityFrameworkCore.Encryption.git
```

We will refer to this directory as `LIBRARY_ROOT` from now on.

## Secrets and Dependency Injection

Afterwards, you need to initialize the encryption settings by adding a key and an initialization vector. If you don't have any yet, you can create
one by running the `KeyGenerator` utility. From within `LIBRARY_ROOT`:

```
    $ dotnet run --project src/EntityFrameworkCore.Encryption.KeyGenerator
```

will output both secrets:

```
    The KeyGenerator can be used to initialize the encryption keys.
    Initialization Vector (IV): XXXXXXXXXXXXXXXXXXXX
    Key: XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    Please paste this keys into your application configuration!
```

You can use these secrets to configure the encryption algorithm. Add the following lines to your `Startup` or `Program`
class:

```
    services.AddDatabaseEncryption(new EncryptionOptions
    {
            InitializationVector = "XXXXXXXXXXXXXXXXXXXX",
            Key = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
    });
```

This extension function adds the necessary services and options to your service provider.

## Preparing entities for encryption
By default, the library won't encrypt any entity. You'll have to specify which entity should be encrypted. You can do so, 
by adding the `[Encrypt]`-Attribute to your entities, e.g.:

```
    [Encrypt]
    public class Post {
        ...
    }
```

That's it! All (scalar) properties of the `Post` class are now marked for being stored encrypted. Please note that this changes
the datatype of all properties in the database to the provider specific `string` representation. But as the mapping
happens in the library, you won't have to worry about that at all.

:rotating_light: **Warning:** There are certain circumstances (e.g. foreign keys) where you don't want to encrypt a property.
Therefore, you can exclude properties by applying the `[ExcludeFromEncryption]`-Attribute to the corresponding property:

```
    [Encrypt]
    public class Post {
        ...
        [ExcludeFromEncryption] public int BlogId { get; set; }
        ...
    }
```

In this way, the `BlogId` is excluded from encryption and in this way the foreign key relationship stays valid.
## DbContext modification
After marking the entities with the encryption attribute, you'll have to make two minor adjustments to your `DbContext`:

First of all, add the `IEncryptionService` to a new or existing constructor. The service will be injected by Dependency 
Injection and needs to be stored in a private field:

```
    public class BloggingContext : DbContext
    {
        private readonly IEncryptionService _service;
        
        ...
        
        public BloggingContext(IEncryptionService service)
        {
               _service = service;
        }
        
        ...
        
    }
```

Finally, add the following line to the `OnModelCreating(ModelBuilder)` method of your `DbContext`:

```
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
          base.OnModelCreating(modelBuilder);
          modelBuilder.ApplyEncryption(_service);
    }
```

The `ApplyEncryption(IEncryptionService)` function will attach the `EncryptionValueConverter` to your all properties
that should be encrypted (based on your selection in the last step). As previously stated, this can change the type
of the column in the database, as all encrypted data is stored as Base64-strings. Therefore, you need to add and 
apply a new migration to your database:

```
    $ dotnet ef migrations add AddEncryptionService
```

That's it! Any new data that is added to your database should now be automatically encrypted. If you want to encrypt
existing data, take a look below.

## Encrypting existing data
The EntityFramework detects that a new ValueConverter has been added and adds `AlterColumn` operations to the new migration,
it does - as far as i know - unfortunately not provide any possibility to migrate unencrypted existing data via a migration.
The `EncryptionValueConverter` handles unencrypted data very tolerant and passes it through without throwing an error, however
you certainly want to encrypt pre-existing data as well.

In order to make this step as simple as possible, I've added the `IEncryptionMigrator` service. This service loops over all
entities in your database and marks them as `Modified`. In this way, the EntityFramework creates an update for each entity in
the database and invokes the `EncryptionValueConverter`.

As you might guess, this is not efficient, especially for large databases. Therefore, you should **not** add it to your application startup
logic but only run it once per database by yourself.

In the beginning, you need an instance of the `IEncryptionMigrator` service:

```
    var migrator = serviceProvider.GetRequiredService<IEncryptionMigrator>();
``` 

Afterwards, you pass in the `DbContext` object of the database that should be encrypted:

```
    migrator.EncryptDatabase(context);
```

That's it. The `EncryptDatabase(DbContext)` function will now automatically encrypt all the data in the database. However,
this function is idempotent. Meaning that it would not encrypt already encrypted data twice.

# Known issues

* At least on sqlite Provider, seeding with `modelBuilder.Entity<>.HasData()` does not work and throws a `NullReferenceException`

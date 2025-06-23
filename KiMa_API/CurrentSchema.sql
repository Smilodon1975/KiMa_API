CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "AspNetRoles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY AUTOINCREMENT,
    "Role" TEXT NOT NULL,
    "CreatedAt" datetime NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "UserName" TEXT NOT NULL,
    "Title" TEXT NULL,
    "Gender" TEXT NULL,
    "Status" TEXT NULL,
    "PhonePrivate" TEXT NULL,
    "PhoneMobile" TEXT NULL,
    "PhoneWork" TEXT NULL,
    "Age" INTEGER NOT NULL,
    "BirthDate" TEXT NULL,
    "Street" TEXT NULL,
    "Zip" TEXT NULL,
    "City" TEXT NULL,
    "Country" TEXT NULL,
    "DataConsent" INTEGER NOT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "FAQs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_FAQs" PRIMARY KEY AUTOINCREMENT,
    "Question" TEXT NOT NULL,
    "Answer" TEXT NOT NULL,
    "SortOrder" INTEGER NOT NULL
);

CREATE TABLE "Feedbacks" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Feedbacks" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NULL,
    "Email" TEXT NULL,
    "Content" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL
);

CREATE TABLE "News" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_News" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Content" TEXT NOT NULL,
    "PublishDate" TEXT NOT NULL
);

CREATE TABLE "NewsletterSubscribers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_NewsletterSubscribers" PRIMARY KEY AUTOINCREMENT,
    "Email" TEXT NOT NULL,
    "IsSubscribed" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" INTEGER NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" INTEGER NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" INTEGER NOT NULL,
    "RoleId" INTEGER NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" INTEGER NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "UserProfiles" (
    "UserId" INTEGER NOT NULL CONSTRAINT "PK_UserProfiles" PRIMARY KEY,
    "VehicleCategory" INTEGER NOT NULL,
    "VehicleDetails" TEXT NULL,
    "Occupation" TEXT NULL,
    "EducationLevel" TEXT NULL,
    "Region" TEXT NULL,
    "Age" INTEGER NULL,
    "IncomeLevel" TEXT NULL,
    "IsInterestedInTechnology" INTEGER NOT NULL,
    "IsInterestedInSports" INTEGER NOT NULL,
    "IsInterestedInEntertainment" INTEGER NOT NULL,
    "IsInterestedInTravel" INTEGER NOT NULL,
    CONSTRAINT "FK_UserProfiles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

INSERT INTO "AspNetRoles" ("Id", "ConcurrencyStamp", "Name", "NormalizedName")
VALUES (1, NULL, 'Admin', 'ADMIN');
SELECT changes();

INSERT INTO "AspNetRoles" ("Id", "ConcurrencyStamp", "Name", "NormalizedName")
VALUES (2, NULL, 'Proband', 'PROBAND');
SELECT changes();


CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250508091549_InitialCreate', '9.0.5');

ALTER TABLE "AspNetUsers" RENAME COLUMN "DataConsent" TO "NewsletterSub";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250515110248_RenameDataConsentToNewsletterSub', '9.0.5');

CREATE TABLE "Projects" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Projects" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL
);

CREATE TABLE "Responses" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Responses" PRIMARY KEY AUTOINCREMENT,
    "ProjectId" INTEGER NOT NULL,
    "RespondentEmail" TEXT NULL,
    "AnswersJson" TEXT NULL,
    "SubmittedAt" TEXT NOT NULL,
    CONSTRAINT "FK_Responses_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Responses_ProjectId" ON "Responses" ("ProjectId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250601193226_AddProjects', '9.0.5');

ALTER TABLE "Projects" ADD "QuestionsJson" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250602110926_AddQuestionsJsonToProject', '9.0.5');

ALTER TABLE "Projects" ADD "Status" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250607175649_AddProjectStatus', '9.0.5');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250623090918_FinalizeProjectsSchema', '9.0.5');

COMMIT;


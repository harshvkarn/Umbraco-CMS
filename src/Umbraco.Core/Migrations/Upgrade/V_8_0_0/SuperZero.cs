﻿namespace Umbraco.Core.Migrations.Upgrade.V_8_0_0
{
    public class SuperZero : MigrationBase
    {
        public SuperZero(IMigrationContext context)
            : base(context)
        { }

        public override void Migrate()
        {
            var exists = Database.Fetch<int>("select id from umbracoUser where id=-1;").Count > 0;
            if (exists) return;

            Database.Execute("set identity_insert umbracoUser on;");
            Database.Execute("update umbracoUser set userLogin = userLogin + '__' where userId=0");
            Database.Execute(@"
                insert into umbracoUser select -1,
                    userDisabled, userNoConsole, userName,  substring(userLogin, 1, len(userLogin) - 2), userPassword, passwordConfig,
	                userEmail, userLanguage, securityStampToken, failedLoginAttempts, lastLockoutDate,
	                lastPasswordChangeDate, lastLoginDate, emailConfirmedDate, invitedDate, 
	                createDate, updateDate, avatar
                from umbracoUser where id=0;");
            Database.Execute("update umbracoUser2UserGroup set userId=-1 where userId=0;");
            Database.Execute("delete from umbracoUser where id=0;");
            Database.Execute("set identity_insert umbracoUser off;");
        }
    }
}
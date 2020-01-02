using System;
using System.Collections.Generic;

namespace DSC.Data
{
    public class FriendSourceFlags
    {
        public bool all { get; set; }
    }

    public class UserSettings
    {
        public int timezone_offset { get; set; }
        public string theme { get; set; }
        public bool stream_notifications_enabled { get; set; }
        public string status { get; set; }
        public bool show_current_game { get; set; }
        public List<object> restricted_guilds { get; set; }
        public bool render_reactions { get; set; }
        public bool render_embeds { get; set; }
        public bool message_display_compact { get; set; }
        public string locale { get; set; }
        public bool inline_embed_media { get; set; }
        public bool inline_attachment_media { get; set; }
        public List<object> guild_positions { get; set; }
        public List<object> guild_folders { get; set; }
        public bool gif_auto_play { get; set; }
        public FriendSourceFlags friend_source_flags { get; set; }
        public int explicit_content_filter { get; set; }
        public bool enable_tts_command { get; set; }
        public bool disable_games_tab { get; set; }
        public bool developer_mode { get; set; }
        public bool detect_platform_accounts { get; set; }
        public bool default_guilds_restricted { get; set; }
        public object custom_status { get; set; }
        public bool convert_emoticons { get; set; }
        public bool contact_sync_enabled { get; set; }
        public bool animate_emoji { get; set; }
        public bool allow_accessibility_detection { get; set; }
        public int afk_timeout { get; set; }
    }

    public class UserGuildSetting
    {
        public bool suppress_everyone { get; set; }
        public bool muted { get; set; }
        public object mute_config { get; set; }
        public bool mobile_push { get; set; }
        public int message_notifications { get; set; }
        public string guild_id { get; set; }
        public List<object> channel_overrides { get; set; }
    }

    public class UserFeedSettings
    {
        public List<object> unsubscribed_users { get; set; }
        public List<object> unsubscribed_games { get; set; }
        public List<object> subscribed_users { get; set; }
        public List<object> subscribed_games { get; set; }
        public List<string> autosubscribed_users { get; set; }
    }

    public class User
    {
        public bool verified { get; set; }
        public string username { get; set; }
        public bool premium { get; set; }
        public object phone { get; set; }
        public bool mobile { get; set; }
        public bool mfa_enabled { get; set; }
        public string id { get; set; }
        public int flags { get; set; }
        public string email { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
    }

    public class User2
    {
        public string username { get; set; }
        public string id { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
    }

    public class Relationship
    {
        public User2 user { get; set; }
        public int type { get; set; }
        public string id { get; set; }
    }

    public class ReadState
    {
        public int mention_count { get; set; }
        public DateTime last_pin_timestamp { get; set; }
        public object last_message_id { get; set; }
        public string id { get; set; }
    }

    public class Recipient
    {
        public string username { get; set; }
        public string id { get; set; }
        public string discriminator { get; set; }
        public bool bot { get; set; }
        public string avatar { get; set; }
    }

    public class PrivateChannel
    {
        public int type { get; set; }
        public List<Recipient> recipients { get; set; }
        public string last_message_id { get; set; }
        public string id { get; set; }
    }

    public class User3
    {
        public string username { get; set; }
        public string id { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
    }

    public class ClientStatus
    {
        public string desktop { get; set; }
    }

    public class Presence
    {
        public User3 user { get; set; }
        public string status { get; set; }
        public long last_modified { get; set; }
        public object game { get; set; }
        public ClientStatus client_status { get; set; }
        public List<object> activities { get; set; }
    }

    public class Notes
    {
    }

    public class User4
    {
        public string id { get; set; }
    }

    public class Emoji
    {
        public string name { get; set; }
        public string id { get; set; }
        public bool? animated { get; set; }
    }

    public class Timestamps
    {
        public object start { get; set; }
    }

    public class Game
    {
        public int type { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public Emoji emoji { get; set; }
        public object created_at { get; set; }
        public string state { get; set; }
        public int? flags { get; set; }
        public Timestamps timestamps { get; set; }
        public string application_id { get; set; }
        public string platform { get; set; }
    }

    public class ClientStatus2
    {
        public string mobile { get; set; }
        public string desktop { get; set; }
        public string web { get; set; }
    }

    public class Presence2
    {
        public User4 user { get; set; }
        public string status { get; set; }
        public Game game { get; set; }
        public ClientStatus2 client_status { get; set; }
        public List<object> activities { get; set; }
    }

    public class User5
    {
        public string username { get; set; }
        public string id { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
        public bool? bot { get; set; }
    }

    public class Member
    {
        public User5 user { get; set; }
        public List<string> roles { get; set; }
        public DateTime? premium_since { get; set; }
        public string nick { get; set; }
        public bool mute { get; set; }
        public DateTime joined_at { get; set; }
        public string hoisted_role { get; set; }
        public bool deaf { get; set; }
    }

    public class Role
    {
        public int position { get; set; }
        public int permissions { get; set; }
        public string name { get; set; }
        public bool mentionable { get; set; }
        public bool managed { get; set; }
        public string id { get; set; }
        public bool hoist { get; set; }
        public int color { get; set; }
    }

    public class Emoji2
    {
        public List<object> roles { get; set; }
        public bool require_colons { get; set; }
        public string name { get; set; }
        public bool managed { get; set; }
        public string id { get; set; }
        public bool available { get; set; }
        public bool animated { get; set; }
    }

    public class Channel
    {
        public int type { get; set; }
        public string topic { get; set; }
        public int rate_limit_per_user { get; set; }
        public int position { get; set; }
        public List<object> permission_overwrites { get; set; }
        public string parent_id { get; set; }
        public bool nsfw { get; set; }
        public string name { get; set; }
        public string last_message_id { get; set; }
        public string id { get; set; }
        public int? user_limit { get; set; }
        public int? bitrate { get; set; }
        public DateTime? last_pin_timestamp { get; set; }
    }

    public class Guild
    {
        public object application_id { get; set; }
        public List<Presence2> presences { get; set; } // Rich Presences of every user in the server.
        public string system_channel_id { get; set; }
        public DateTime joined_at { get; set; }
        public bool large { get; set; }
        public int afk_timeout { get; set; }
        public int premium_subscription_count { get; set; }
        public int member_count { get; set; } // Amount of members in the guild
        public object vanity_url_code { get; set; } // the "Vanity" url to a server (if applicable)
        public string afk_channel_id { get; set; }
        public string region { get; set; }
        public List<Member> members { get; set; }
        public List<Role> roles { get; set; }
        public int explicit_content_filter { get; set; } // Content filter level.
        public List<Emoji2> emojis { get; set; }
        public int default_message_notifications { get; set; }
        public string name { get; set; }
        public string preferred_locale { get; set; }
        public int premium_tier { get; set; } // Server boost!
        public object rules_channel_id { get; set; }
        public int system_channel_flags { get; set; }
        public object discovery_splash { get; set; }
        public List<object> voice_states { get; set; }
        public object splash { get; set; }
        public List<object> features { get; set; }
        public List<Channel> channels { get; set; }
        public string icon { get; set; }
        public bool lazy { get; set; }
        public int verification_level { get; set; }
        public object description { get; set; }
        public int mfa_level { get; set; }
        public object banner { get; set; }
        public string id { get; set; }
        public string owner_id { get; set; }
    }

    public class Personalization
    {
        public bool consented { get; set; }
    }

    public class Consents
    {
        public Personalization personalization { get; set; }
    }

    public class D
    {
        public int v { get; set; }
        public UserSettings user_settings { get; set; }
        public List<UserGuildSetting> user_guild_settings { get; set; }
        public UserFeedSettings user_feed_settings { get; set; }
        public User user { get; set; }
        public object tutorial { get; set; }
        public string session_id { get; set; }
        public List<Relationship> relationships { get; set; }
        public List<ReadState> read_state { get; set; }
        public List<PrivateChannel> private_channels { get; set; }
        public List<Presence> presences { get; set; }
        public Notes notes { get; set; }
        public List<Guild> guilds { get; set; }
        public List<List<object>> guild_experiments { get; set; }
        public int friend_suggestion_count { get; set; }
        public List<List<object>> experiments { get; set; }
        public Consents consents { get; set; }
        public List<object> connected_accounts { get; set; }
        public string analytics_token { get; set; }
        public List<string> _trace { get; set; }
    }

    public class RootObject
    {
        public string t { get; set; }
        public string s { get; set; }
        public int op { get; set; }
        public D d { get; set; }
    }
}

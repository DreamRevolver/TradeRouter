
// ReSharper disable UnassignedReadonlyField

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeInternal
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace
namespace TelegramBotAPI.Types
{

    public interface ITelegramBotAPIType { }

    public readonly struct CallbackGame : ITelegramBotAPIType { }
    public readonly struct MessageId : ITelegramBotAPIType
    {
        public readonly long? message_id;
    }
    public readonly struct KeyboardButtonPollType : ITelegramBotAPIType
    {
        public readonly string type;
    }
    public readonly struct PollOption : ITelegramBotAPIType
    {
        public readonly string text;
        public readonly long? voter_count;
    }
    public readonly struct ResponseParameters : ITelegramBotAPIType
    {
        public readonly long? migrate_to_chat_id;
        public readonly long? retry_after;
    }
    public readonly struct Dice : ITelegramBotAPIType
    {
        public readonly string emoji;
        public readonly long? value;
    }
    public readonly struct ReplyKeyboardRemove : ITelegramBotAPIType
    {
        public readonly bool? remove_keyboard;
        public readonly bool? selective;
    }
    public readonly struct ForceReply : ITelegramBotAPIType
    {
        public readonly bool? force_reply;
        public readonly bool? selective;
    }
    public readonly struct BotCommand : ITelegramBotAPIType
    {
        public readonly string command;
        public readonly string description;
    }
    public readonly struct EncryptedCredentials : ITelegramBotAPIType
    {
        public readonly string data;
        public readonly string hash;
        public readonly string secret;
    }
    public readonly struct PassportFile : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly long? file_size;
        public readonly long? file_date;
    }
    public readonly struct ChatPhoto : ITelegramBotAPIType
    {
        public readonly string small_file_id;
        public readonly string small_file_unique_id;
        public readonly string big_file_id;
        public readonly string big_file_unique_id;
    }
    public readonly struct MaskPosition : ITelegramBotAPIType
    {
        public readonly string point;
        public readonly double? x_shift;
        public readonly double? y_shift;
        public readonly double? scale;
    }
    public readonly struct LoginUrl : ITelegramBotAPIType
    {
        public readonly string url;
        public readonly string forward_text;
        public readonly string bot_username;
        public readonly bool? request_write_access;
    }
    public readonly struct File : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly long? file_size;
        public readonly string file_path;
    }
    public readonly struct PhotoSize : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly long? width;
        public readonly long? height;
        public readonly long? file_size;
    }
    public readonly struct Voice : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly long? duration;
        public readonly string mime_type;
        public readonly long? file_size;
    }
    public readonly struct Invoice : ITelegramBotAPIType
    {
        public readonly string title;
        public readonly string description;
        public readonly string start_parameter;
        public readonly string currency;
        public readonly string total_amount;
    }
    public readonly struct Contact : ITelegramBotAPIType
    {
        public readonly string phone_number;
        public readonly string first_name;
        public readonly string last_name;
        public readonly long? user_id;
        public readonly string vcard;
    }
    public readonly struct Location : ITelegramBotAPIType
    {
        public readonly double? longitude;
        public readonly double? latitude;
        public readonly double? horizontal_accuracy;
        public readonly long? live_period;
        public readonly long? heading;
        public readonly long? proximity_alert_radius;
    }
    public readonly struct ShippingAddress : ITelegramBotAPIType
    {
        public readonly string country_code;
        public readonly string state;
        public readonly string city;
        public readonly string street_line1;
        public readonly string street_line2;
        public readonly string post_code;
    }
    public readonly struct ChatPermissions : ITelegramBotAPIType
    {
        public readonly bool? can_send_messages;
        public readonly bool? can_send_media_messages;
        public readonly bool? can_send_polls;
        public readonly bool? can_send_other_messages;
        public readonly bool? can_add_web_page_previews;
        public readonly bool? can_change_info;
        public readonly bool? can_invite_users;
        public readonly bool? can_pin_messages;
    }
    public readonly struct User : ITelegramBotAPIType
    {
        public readonly long? id;
        public readonly bool? is_bot;
        public readonly string first_name;
        public readonly string last_name;
        public readonly string username;
        public readonly string language_code;
        public readonly bool? can_join_groups;
        public readonly bool? can_read_all_group_messages;
        public readonly bool? supports_inline_queries;
    }

    public readonly struct ChatLocation : ITelegramBotAPIType
    {
        public readonly Location? location;
        public readonly string address;
    }
    public readonly struct UserProfilePhotos : ITelegramBotAPIType
    {
        public readonly long? total_count;

        public readonly PhotoSize[] photos;
    }
    public readonly struct ProximityAlertTriggered : ITelegramBotAPIType
    {
        public readonly User? traveler;
        public readonly User? watcher;
        public readonly long? distance;
    }
    public readonly struct PollAnswer : ITelegramBotAPIType
    {
        public readonly string poll_id;
        public readonly User? user;

        public readonly long[] options_ids;
    }
    public readonly struct KeyboardButton : ITelegramBotAPIType
    {
        public readonly string text;
        public readonly bool? request_contact;
        public readonly bool? request_location;
        public readonly KeyboardButtonPollType? request_poll;
    }
    public readonly struct ShippingQuery : ITelegramBotAPIType
    {
        public readonly string id;
        public readonly User? from;
        public readonly string invoice_payload;
        public readonly ShippingAddress? shipping_address;
    }
    public readonly struct OrderInfo : ITelegramBotAPIType
    {
        public readonly string name;
        public readonly string phone_number;
        public readonly string email;
        public readonly ShippingAddress? shipping_address;
    }
    public readonly struct ChosenInlineResult : ITelegramBotAPIType
    {
        public readonly string result_id;
        public readonly User? from;
        public readonly Location? location;
        public readonly string inline_message_id;
        public readonly string query;
    }
    public readonly struct InlineQuery : ITelegramBotAPIType
    {
        public readonly string id;
        public readonly User? from;
        public readonly Location? location;
        public readonly string query;
        public readonly string offset;
    }
    public readonly struct MessageEntity : ITelegramBotAPIType
    {
        public readonly string type;
        public readonly long? offset;
        public readonly long? length;
        public readonly string url;
        public readonly User? user;
        public readonly string language;
    }
    public readonly struct Document : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly PhotoSize? thumb;
        public readonly string file_name;
        public readonly string mime_type;
        public readonly long? file_size;
    }
    public readonly struct VideoNote : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly long? length;
        public readonly long? duration;
        public readonly PhotoSize? thumb;
        public readonly long? file_size;
    }
    public readonly struct Venue : ITelegramBotAPIType
    {
        public readonly Location? location;
        public readonly string title;
        public readonly string address;
        public readonly string foursquare_id;
        public readonly string foursquare_type;
        public readonly string google_place_id;
        public readonly string google_place_type;
    }
    public readonly struct InlineKeyboardButton : ITelegramBotAPIType
    {
        public readonly string text;
        public readonly string url;
        public readonly LoginUrl? login_url;
        public readonly string callback_data;
        public readonly string switch_inline_query;
        public readonly string switch_inline_query_current_chat;
        public readonly CallbackGame? callback_game;
        public readonly bool? pay;
    }
    public readonly struct Animation : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly long? width;
        public readonly long? height;
        public readonly long? duration;
        public readonly PhotoSize? thumb;
        public readonly string file_name;
        public readonly string mime_type;
        public readonly long? file_size;
    }
    public readonly struct Audio : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly long? duration;
        public readonly string performer;
        public readonly string title;
        public readonly string file_name;
        public readonly string mime_type;
        public readonly long? file_size;
        public readonly PhotoSize? thumb;
    }
    public readonly struct Video : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly long? width;
        public readonly long? height;
        public readonly long? duration;
        public readonly PhotoSize? thumb;
        public readonly string file_name;
        public readonly string mime_type;
        public readonly long? file_size;
    }
    public readonly struct Sticker : ITelegramBotAPIType
    {
        public readonly string file_id;
        public readonly string file_unique_id;
        public readonly long? width;
        public readonly long? height;
        public readonly bool? is_animated;
        public readonly PhotoSize? thumb;
        public readonly string emoji;
        public readonly string set_name;
        public readonly MaskPosition? mask_position;
        public readonly long? file_size;
    }
    public readonly struct EncryptedPassportElement : ITelegramBotAPIType
    {
        public readonly string type;
        public readonly string data;
        public readonly string phone_number;
        public readonly string email;
        public readonly PassportFile[] files;
        public readonly PassportFile? front_side;
        public readonly PassportFile? reverse_side;
        public readonly PassportFile? selfie;
        public readonly PassportFile[] translation;
        public readonly string hash;
    }
    public readonly struct ChatMember : ITelegramBotAPIType
    {
        public readonly User? user;
        public readonly string status;
        public readonly string custom_title;
        public readonly bool? is_anonymous;
        public readonly bool? can_be_edited;
        public readonly bool? can_post_messages;
        public readonly bool? can_edit_messages;
        public readonly bool? can_delete_messages;
        public readonly bool? can_restrict_members;
        public readonly bool? can_promote_members;
        public readonly bool? can_change_info;
        public readonly bool? can_invite_users;
        public readonly bool? can_pin_messages;
        public readonly bool? is_member;
        public readonly bool? can_send_messages;
        public readonly bool? can_send_media_messages;
        public readonly bool? can_send_polls;
        public readonly bool? can_send_other_messages;
        public readonly bool? can_add_web_page_previews;
        public readonly long? until_date;
    }

    public readonly struct InlineKeyboardMarkup : ITelegramBotAPIType
    {

        public readonly InlineKeyboardButton[][] inline_keyboard;
    }
    public readonly struct PassportData : ITelegramBotAPIType
    {

        public readonly EncryptedPassportElement[] data;
        public readonly EncryptedCredentials? credentials;
    }
    public readonly struct ReplyKeyboardMarkup : ITelegramBotAPIType
    {

        public readonly KeyboardButton[][] keyboard;
        public readonly bool? resize_keyboard;
        public readonly bool? one_time_keyboard;
        public readonly bool? selective;
    }
    public readonly struct Game : ITelegramBotAPIType
    {
        public readonly string title;
        public readonly string description;

        public readonly PhotoSize[] photo;
        public readonly string text;
        public readonly MessageEntity[] text_entities;
        public readonly Animation? animation;
    }
    public readonly struct SuccessfulPayment : ITelegramBotAPIType
    {
        public readonly string currency;
        public readonly long? total_amount;
        public readonly string invoice_payload;
        public readonly string shipping_option_id;
        public readonly OrderInfo? order_info;
        public readonly string telegram_payment_charge_id;
        public readonly string provider_payment_charge_id;
    }
    public readonly struct PreCheckoutQuery : ITelegramBotAPIType
    {
        public readonly string id;
        public readonly User? from;
        public readonly string currency;
        public readonly long? total_amount;
        public readonly string invoice_payload;
        public readonly string shipping_option_id;
        public readonly OrderInfo? order_info;
    }
    public readonly struct Poll : ITelegramBotAPIType
    {
        public readonly string id;
        public readonly string question;

        public readonly PollOption[] options;
        public readonly long? total_voter_count;
        public readonly bool? is_closed;
        public readonly bool? is_anonymous;
        public readonly string type;
        public readonly bool? allows_multiple_answers;
        public readonly long? correct_option_id;
        public readonly string explanation;
        public readonly MessageEntity[] explanation_entities;
        public readonly long? open_period;
        public readonly long? close_date;
    }

    public readonly struct CallbackQuery : ITelegramBotAPIType
    {
        public readonly string id;
        public readonly User? from;
        public readonly Message message;
        public readonly string inline_message_id;
        public readonly string chat_instance;
        public readonly string data;
        public readonly string game_short_name;
    }
    public readonly struct Update : ITelegramBotAPIType
    {
        public readonly long? update_id;
        public readonly Message message;
        public readonly Message edited_message;
        public readonly Message channel_post;
        public readonly Message edited_channel_post;
        public readonly InlineQuery? inline_query;
        public readonly ChosenInlineResult? chosen_inline_result;
        public readonly CallbackQuery? callback_query;
        public readonly ShippingQuery? shipping_query;
        public readonly PreCheckoutQuery? pre_checkout_query;
        public readonly Poll? poll;
        public readonly PollAnswer? poll_answer;
    }

    public class Chat : ITelegramBotAPIType
    {
        
        public long? id;
        
        public string type;
        public string title;
        public string username;
        public string first_name;
        public string last_name;
        public ChatPhoto? photo;
        public string bio;
        public string description;
        public string invite_link;
        public Message pinned_message;
        public ChatPermissions? permissions;
        public long? slow_mode_delay;
        public string sticker_set_name;
        public bool? can_set_sticker_set;
        public long? linked_chat_id;
        public ChatLocation? location;
    }
    public class Message : ITelegramBotAPIType
    {
        
        public long? message_id;
        public User? from;
        public Chat sender_chat;
        public long? date;
        
        public Chat chat;
        public User? forward_from;
        public Chat forward_from_chat;
        public long? forward_from_message_id;
        public string forward_signature;
        public string forward_sender_name;
        public long? forward_date;
        public Message reply_to_message;
        public User? via_bot;
        public long? edit_date;
        public string media_group_id;
        public string author_signature;
        public string text;
        public MessageEntity[] entities;
        public Animation? animation;
        public Audio? audio;
        public Document? document;
        public PhotoSize[] photo;
        public Sticker? sticker;
        public Video? video;
        public VideoNote? video_note;
        public Voice? voice;
        public string caption;
        public MessageEntity? caption_entities;
        public Contact? contact;
        public Dice? dice;
        public Game? game;
        public Poll? poll;
        public Venue? venue;
        public Location? location;
        public User[] new_chat_members;
        public User? left_chat_member;
        public string new_chat_title;
        public PhotoSize[] new_chat_photo;
        public bool? delete_chat_photo;
        public bool? group_chat_created;
        public bool? supergroup_chat_created;
        public bool? channel_chat_created;
        public long? migrate_to_chat_id;
        public long? migrate_from_chat_id;
        public Message pinned_message;
        public Invoice? invoice;
        public SuccessfulPayment? successful_payment;
        public string connected_website;
        public PassportData? passport_data;
        public ProximityAlertTriggered? proximity_alert_triggered;
        public InlineKeyboardMarkup? reply_markup;
    }

}

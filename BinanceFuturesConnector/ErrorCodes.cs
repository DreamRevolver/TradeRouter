using Utf8Json;

namespace BinanceFuturesConnector
{

	public class BinanceErrorCodeJsonFormatter : IJsonFormatter<ErrorCodeWrapper>
	{
		public void Serialize(ref JsonWriter writer, ErrorCodeWrapper value, IJsonFormatterResolver formatterResolver)
			=> writer.WriteInt32((int)value.value);

		public ErrorCodeWrapper Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
			=> new ErrorCodeWrapper
			{
				value = (ErrorCode)reader.ReadInt32()
			};
	}

	public enum ErrorCode
	{
		UNKNOWN = -1000,
		DISCONNECTED = -1001,
		UNAUTHORIZED = -1002,
		TOO_MANY_REQUESTS = -1003,
		SERVER_BUSY = -1004,
		DUPLICATE_IP = -1004,
		NO_SUCH_IP = -1005,
		UNEXPECTED_RESP = -1006,
		TIMEOUT = -1007,
		ERROR_MSG_RECEIVED = -1010,
		NON_WHITE_LIST = -1011,
		INVALID_MESSAGE = -1013,
		UNKNOWN_ORDER_COMPOSITION = -1014,
		TOO_MANY_ORDERS = -1015,
		SERVICE_SHUTTING_DOWN = -1016,
		UNSUPPORTED_OPERATION = -1020,
		INVALID_TIMESTAMP = -1021,
		INVALID_SIGNATURE = -1022,
		START_TIME_GREATER_THAN_END_TIME = -1023,
		Not = -1099,
		ILLEGAL_CHARS = -1100,
		TOO_MANY_PARAMETERS = -1101,
		MANDATORY_PARAM_EMPTY_OR_MALFORMED = -1102,
		UNKNOWN_PARAM = -1103,
		UNREAD_PARAMETERS = -1104,
		PARAM_EMPTY = -1105,
		PARAM_NOT_REQUIRED = -1106,
		BAD_ASSET = -1108,
		BAD_ACCOUNT = -1109,
		BAD_INSTRUMENT_TYPE = -1110,
		BAD_PRECISION = -1111,
		NO_DEPTH = -1112,
		WITHDRAW_NOT_NEGATIVE = -1113,
		TIF_NOT_REQUIRED = -1114,
		INVALID_TIF = -1115,
		INVALID_ORDER_TYPE = -1116,
		INVALID_SIDE = -1117,
		EMPTY_NEW_CL_ORD_ID = -1118,
		EMPTY_ORG_CL_ORD_ID = -1119,
		BAD_INTERVAL = -1120,
		BAD_SYMBOL = -1121,
		INVALID_LISTEN_KEY = -1125,
		MORE_THAN_XX_HOURS = -1127,
		OPTIONAL_PARAMS_BAD_COMBO = -1128,
		INVALID_PARAMETER = -1130,
		BAD_RECV_WINDOW = -1131,
		INVALID_NEW_ORDER_RESP_TYPE = -1136,
		NEW_ORDER_REJECTED = -2010,
		CANCEL_REJECTED = -2011,
		NO_SUCH_ORDER = -2013,
		BAD_API_KEY_FMT = -2014,
		REJECTED_MBX_KEY = -2015,
		NO_TRADING_WINDOW = -2016,
		BALANCE_NOT_SUFFICIENT = -2018,
		MARGIN_NOT_SUFFICIEN = -2019,
		UNABLE_TO_FILL = -2020,
		ORDER_WOULD_IMMEDIATELY_TRIGGER = -2021,
		REDUCE_ONLY_REJECT = -2022,
		USER_IN_LIQUIDATION = -2023,
		POSITION_NOT_SUFFICIENT = -2024,
		MAX_OPEN_ORDER_EXCEEDED = -2025,
		REDUCE_ONLY_ORDER_TYPE_NOT_SUPPORTED = -2026,
		MAX_LEVERAGE_RATIO = -2027,
		MIN_LEVERAGE_RATIO = -2028,
		INNER_FAILURE = -3000,
		NEED_ENABLE_2FA = -3001,
		ASSET_DEFICIENCY = -3002,
		NO_OPENED_MARGIN_ACCOUNT = -3003,
		TRADE_NOT_ALLOWED = -3004,
		TRANSFER_OUT_NOT_ALLOWED = -3005,
		EXCEED_MAX_BORROWABLE = -3006,
		HAS_PENDING_TRANSACTION = -3007,
		BORROW_NOT_ALLOWED = -3008,
		ASSET_NOT_MORTGAGEABLE = -3009,
		REPAY_NOT_ALLOWED = -3010,
		BAD_DATE_RANGE = -3011,
		ASSET_ADMIN_BAN_BORROW = -3012,
		LT_MIN_BORROWABLE = -3013,
		ACCOUNT_BAN_BORROW = -3014,
		REPAY_EXCEED_LIABILITY = -3015,
		LT_MIN_REPAY = -3016,
		ASSET_ADMIN_BAN_MORTGAGE = -3017,
		ACCOUNT_BAN_MORTGAGE = -3018,
		ACCOUNT_BAN_ROLLOUT = -3019,
		EXCEED_MAX_ROLLOUT = -3020,
		PAIR_ADMIN_BAN_TRADE = -3021,
		ACCOUNT_BAN_TRADE = -3022,
		WARNING_MARGIN_LEVEL = -3023,
		FEW_LIABILITY_LEFT = -3024,
		INVALID_EFFECTIVE_TIME = -3025,
		VALIDATION_FAILED = -3026,
		NOT_VALID_MARGIN_ASSET = -3027,
		NOT_VALID_MARGIN_PAIR = -3028,
		TRANSFER_FAILED = -3029,
		ACCOUNT_BAN_REPAY = -3036,
		PNL_CLEARING = -3037,
		LISTEN_KEY_NOT_FOUND = -3038,
		BALANCE_NOT_CLEARED = -3041,
		PRICE_INDEX_NOT_FOUND = -3042,
		TRANSFER_IN_NOT_ALLOWED = -3043,
		SYSTEM_BUSY = -3044,
		NOT_WHITELIST_USER = -3999,
		INVALID_ORDER_STATUS = -4000,
		PRICE_LESS_THAN_ZERO = -4001,
		CAPITAL_INVALID = -4001,
		PRICE_GREATER_THAN_MAX_PRICE = -4002,
		CAPITAL_IG = -4002,
		QTY_LESS_THAN_ZERO = -4003,
		CAPITAL_IEV = -4003,
		QTY_LESS_THAN_MIN_QTY = -4004,
		CAPITAL_UA = -4004,
		QTY_GREATER_THAN_MAX_QTY = -4005,
		CAPAITAL_TOO_MANY_REQUEST = -4005,
		STOP_PRICE_LESS_THAN_ZERO = -4006,
		CAPITAL_ONLY_SUPPORT_PRIMARY_ACCOUNT = -4006,
		STOP_PRICE_GREATER_THAN_MAX_PRICE = -4007,
		CAPITAL_ADDRESS_VERIFICATION_NOT_PASS = -4007,
		TICK_SIZE_LESS_THAN_ZERO = -4008,
		CAPITAL_ADDRESS_TAG_VERIFICATION_NOT_PASS = -4008,
		MAX_PRICE_LESS_THAN_MIN_PRICE = -4009,
		MAX_QTY_LESS_THAN_MIN_QTY = -4010,
		CAPITAL_WHITELIST_EMAIL_CONFIRM = -4010,
		STEP_SIZE_LESS_THAN_ZERO = -4011,
		CAPITAL_WHITELIST_EMAIL_EXPIRED = -4011,
		MAX_NUM_ORDERS_LESS_THAN_ZERO = -4012,
		CAPITAL_WHITELIST_CLOSE = -4012,
		PRICE_LESS_THAN_MIN_PRICE = -4013,
		CAPITAL_WITHDRAW_2FA_VERIFY = -4013,
		PRICE_NOT_INCREASED_BY_TICK_SIZE = -4014,
		CAPITAL_WITHDRAW_LOGIN_DELAY = -4014,
		INVALID_CL_ORD_ID_LEN = -4015,
		CAPITAL_WITHDRAW_RESTRICTED_MINUTE = -4015,
		PRICE_HIGHTER_THAN_MULTIPLIER_UP = -4016,
		CAPITAL_WITHDRAW_RESTRICTED_PASSWORD = -4016,
		MULTIPLIER_UP_LESS_THAN_ZERO = -4017,
		CAPITAL_WITHDRAW_RESTRICTED_UNBIND_2FA = -4017,
		MULTIPLIER_DOWN_LESS_THAN_ZERO = -4018,
		CAPITAL_WITHDRAW_ASSET_NOT_EXIST = -4018,
		COMPOSITE_SCALE_OVERFLOW = -4019,
		CAPITAL_WITHDRAW_ASSET_PROHIBIT = -4019,
		TARGET_STRATEGY_INVALID = -4020,
		INVALID_DEPTH_LIMIT = -4021,
		CAPITAL_WITHDRAW_AMOUNT_MULTIPLE = -4021,
		WRONG_MARKET_STATUS = -4022,
		CAPITAL_WITHDRAW_MIN_AMOUNT = -4022,
		QTY_NOT_INCREASED_BY_STEP_SIZE = -4023,
		CAPITAL_WITHDRAW_MAX_AMOUNT = -4023,
		PRICE_LOWER_THAN_MULTIPLIER_DOWN = -4024,
		CAPITAL_WITHDRAW_USER_NO_ASSET = -4024,
		MULTIPLIER_DECIMAL_LESS_THAN_ZERO = -4025,
		CAPITAL_WITHDRAW_USER_ASSET_LESS_THAN_ZERO = -4025,
		COMMISSION_INVALID = -4026,
		CAPITAL_WITHDRAW_USER_ASSET_NOT_ENOUGH = -4026,
		INVALID_ACCOUNT_TYPE = -4027,
		CAPITAL_WITHDRAW_GET_TRAN_ID_FAILURE = -4027,
		INVALID_LEVERAGE = -4028,
		CAPITAL_WITHDRAW_MORE_THAN_FEE = -4028,
		INVALID_TICK_SIZE_PRECISION = -4029,
		CAPITAL_WITHDRAW_NOT_EXIST = -4029,
		INVALID_STEP_SIZE_PRECISION = -4030,
		CAPITAL_WITHDRAW_CONFIRM_SUCCESS = -4030,
		INVALID_WORKING_TYPE = -4031,
		CAPITAL_WITHDRAW_CANCEL_FAILURE = -4031,
		EXCEED_MAX_CANCEL_ORDER_SIZE = -4032,
		CAPITAL_WITHDRAW_CHECKSUM_VERIFY_FAILURE = -4032,
		INSURANCE_ACCOUNT_NOT_FOUND = -4033,
		CAPITAL_WITHDRAW_ILLEGAL_ADDRESS = -4033,
		CAPITAL_WITHDRAW_ADDRESS_CHEAT = -4034,
		CAPITAL_WITHDRAW_NOT_WHITE_ADDRESS = -4035,
		CAPITAL_WITHDRAW_NEW_ADDRESS = -4036,
		CAPITAL_WITHDRAW_RESEND_EMAIL_FAIL = -4037,
		CAPITAL_WITHDRAW_RESEND_EMAIL_TIME_OUT = -4038,
		CAPITAL_USER_EMPTY = -4039,
		CAPITAL_NO_CHARGE = -4040,
		CAPITAL_MINUTE_TOO_SMALL = -4041,
		CAPITAL_CHARGE_NOT_RESET = -4042,
		CAPITAL_ADDRESS_TOO_MUCH = -4043,
		INVALID_BALANCE_TYPE = -4044,
		CAPITAL_BLACKLIST_COUNTRY_GET_ADDRESS = -4044,
		MAX_STOP_ORDER_EXCEEDED = -4045,
		CAPITAL_GET_ASSET_ERROR = -4045,
		NO_NEED_TO_CHANGE_MARGIN_TYPE = -4046,
		CAPITAL_AGREEMENT_NOT_CONFIRMED = -4046,
		THERE_EXISTS_OPEN_ORDERS = -4047,
		CAPITAL_DATE_INTERVAL_LIMIT = -4047,
		THERE_EXISTS_QUANTITY = -4048,
		ADD_ISOLATED_MARGIN_REJECT = -4049,
		CROSS_BALANCE_INSUFFICIENT = -4050,
		ISOLATED_BALANCE_INSUFFICIENT = -4051,
		NO_NEED_TO_CHANGE_AUTO_ADD_MARGIN = -4052,
		AUTO_ADD_CROSSED_MARGIN_REJECT = -4053,
		ADD_ISOLATED_MARGIN_NO_POSITION_REJECT = -4054,
		AMOUNT_MUST_BE_POSITIVE = -4055,
		INVALID_API_KEY_TYPE = -4056,
		INVALID_RSA_PUBLIC_KEY = -4057,
		MAX_PRICE_TOO_LARGE = -4058,
		NO_NEED_TO_CHANGE_POSITION_SIDE = -4059,
		INVALID_POSITION_SIDE = -4060,
		POSITION_SIDE_NOT_MATCH = -4061,
		REDUCE_ONLY_CONFLICT = -4062,
		INVALID_OPTIONS_REQUEST_TYPE = -4063,
		INVALID_OPTIONS_TIME_FRAME = -4064,
		INVALID_OPTIONS_AMOUNT = -4065,
		INVALID_OPTIONS_EVENT_TYPE = -4066,
		POSITION_SIDE_CHANGE_EXISTS_OPEN_ORDERS = -4067,
		POSITION_SIDE_CHANGE_EXISTS_QUANTITY = -4068,
		INVALID_OPTIONS_PREMIUM_FEE = -4069,
		INVALID_CL_OPTIONS_ID_LEN = -4070,
		INVALID_OPTIONS_DIRECTION = -4071,
		OPTIONS_PREMIUM_NOT_UPDATE = -4072,
		OPTIONS_PREMIUM_INPUT_LESS_THAN_ZERO = -4073,
		OPTIONS_AMOUNT_BIGGER_THAN_UPPER = -4074,
		OPTIONS_PREMIUM_OUTPUT_ZERO = -4075,
		OPTIONS_PREMIUM_TOO_DIFF = -4076,
		OPTIONS_PREMIUM_REACH_LIMIT = -4077,
		OPTIONS_COMMON_ERROR = -4078,
		INVALID_OPTIONS_ID = -4079,
		OPTIONS_USER_NOT_FOUND = -4080,
		OPTIONS_NOT_FOUND = -4081,
		INVALID_BATCH_PLACE_ORDER_SIZE = -4082,
		PLACE_BATCH_ORDERS_FAIL = -4083,
		UPCOMING_METHOD = -4084,
		INVALID_NOTIONAL_LIMIT_COEF = -4085,
		INVALID_PRICE_SPREAD_THRESHOLD = -4086,
		REDUCE_ONLY_ORDER_PERMISSION = -4087,
		NO_PLACE_ORDER_PERMISSION = -4088,
		INVALID_CONTRACT_TYPE = -4104,
		INVALID_CLIENT_TRAN_ID_LEN = -4114,
		DUPLICATED_CLIENT_TRAN_ID = -4115,
		REDUCE_ONLY_MARGIN_CHECK_FAILED = -4118,
		MARKET_ORDER_REJECT = -4131,
		INVALID_ACTIVATION_PRICE = -4135,
		QUANTITY_EXISTS_WITH_CLOSE_POSITION = -4137,
		REDUCE_ONLY_MUST_BE_TRUE = -4138,
		ORDER_TYPE_CANNOT_BE_MKT = -4139,
		INVALID_OPENING_POSITION_STATUS = -4140,
		SYMBOL_ALREADY_CLOSED = -4141,
		STRATEGY_INVALID_TRIGGER_PRICE = -4142,
		INVALID_PAIR = -4144,
		ISOLATED_LEVERAGE_REJECT_WITH_POSITION = -4161,
		MIN_NOTIONAL = -4164,
		INVALID_TIME_INTERVAL = -4165,
		PRICE_HIGHTER_THAN_STOP_MULTIPLIER_UP = -4183,
		PRICE_LOWER_THAN_STOP_MULTIPLIER_DOWN = -4184,
		ASSET_DRIBBLET_CONVERT_SWITCH_OFF = -5001,
		ASSET_ASSET_NOT_ENOUGH = -5002,
		ASSET_USER_HAVE_NO_ASSET = -5003,
		USER_OUT_OF_TRANSFER_FLOAT = -5004,
		USER_ASSET_AMOUNT_IS_TOO_LOW = -5005,
		USER_CAN_NOT_REQUEST_IN_24_HOURS = -5006,
		AMOUNT_OVER_ZERO = -5007,
		ASSET_WITHDRAW_WITHDRAWING_NOT_ENOUGH = -5008,
		PRODUCT_NOT_EXIST = -5009,
		TRANSFER_FAIL = -5010,
		FUTURE_ACCT_NOT_EXIST = -5011,
		TRANSFER_PENDING = -5012,
		FUTURE_ACCT_OR_SUBRELATION_NOT_EXIST = -5012,
		PARENT_SUB_HAVE_NO_RELATION = -5021,
		DAILY_PRODUCT_NOT_EXIST = -6001,
		DAILY_PRODUCT_NOT_ACCESSIBLE = -6003,
		DAILY_PRODUCT_NOT_PURCHASABLE = -6004,
		DAILY_LOWER_THAN_MIN_PURCHASE_LIMIT = -6005,
		DAILY_REDEEM_AMOUNT_ERROR = -6006,
		DAILY_REDEEM_TIME_ERROR = -6007,
		DAILY_PRODUCT_NOT_REDEEMABLE = -6008,
		REQUEST_FREQUENCY_TOO_HIGH = -6009,
		EXCEEDED_USER_PURCHASE_LIMIT = -6011,
		BALANCE_NOT_ENOUGH = -6012,
		PURCHASING_FAILED = -6013,
		UPDATE_FAILED = -6014,
		EMPTY_REQUEST_BODY = -6015,
		PARAMS_ERR = -6016,
		NOT_IN_WHITELIST = -6017,
		ASSET_NOT_ENOUGH = -6018,
		PENDING = -6019,
		PROJECT_NOT_EXISTS = -6020,
		FUTURES_BAD_DATE_RANGE = -7001,
		FUTURES_BAD_TYPE = -7002,
		REPAY_CHECK_BEYOND_LIABILITY = -10017,
		BLVT_FORBID_REDEEM = -13000,
		BLVT_EXCEED_DAILY_LIMIT = -13001,
		BLVT_EXCEED_TOKEN_DAILY_LIMIT = -13002,
		BLVT_FORBID_PURCHASE = -13003,
		BLVT_EXCEED_DAILY_PURCHASE_LIMIT = -13004,
		BLVT_EXCEED_TOKEN_DAILY_PURCHASE_LIMIT = -13005,
		BLVT_PURCHASE_LESS_MIN_AMOUNT = -13006,
		BLVT_PURCHASE_AGREEMENT_NOT_SIGN = -13007,
	}

	[JsonFormatter(typeof(BinanceErrorCodeJsonFormatter))]
	public struct ErrorCodeWrapper
	{
		public ErrorCode value;
		public override string ToString()
			=> value.ToString();
	}


}
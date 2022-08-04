namespace Communication.Redis.Models
{
	internal enum RedisRespType
	{
		SimpleString = '+',
		Error = '-',
		Integer = ':',
		BulkString = '$',
		Array = '*'
	}
}

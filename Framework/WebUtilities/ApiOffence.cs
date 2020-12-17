using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.WebUtilities
{
	/// <summary>
	/// Api Offense 
	/// </summary>
	public struct ApiOffence
	{
		public string ErrorCode { get; set; }
		public ApiOffence(string errCode)
		{
			ErrorCode = errCode;
		}

		public static implicit operator ApiOffence(string errCode)
		{
			return new ApiOffence(errCode);
		}
	}
}

using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class TitleMapModel<TName, TValue>
	{
        public TName name { get; set; }

        public TValue value { get; set; }
	}
}

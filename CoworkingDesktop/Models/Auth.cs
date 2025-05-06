using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkingDesktop.Models
{
    public class TokensDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }

}


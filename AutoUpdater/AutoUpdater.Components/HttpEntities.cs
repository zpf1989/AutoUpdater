using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Components
{
    public class HttpHeader
    {
        private string _contentType = "text/html;charset=utf-8";
        public string contentType
        {
            get { return _contentType; }
            set
            {
                _contentType = value;
            }
        }
        private string _accept = "text/html,application/xhtml+xml,application/xml,application/json;q=0.9,*/*;q=0.8";
        public string accept
        {
            get { return _accept; }
            set
            {
                _accept = value;
            }
        }
        private string _userAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        public string userAgent
        {
            get { return _userAgent; }
            set
            {
                _userAgent = value;
            }
        }
        private string _method = "GET";
        public string method
        {
            get { return _method; }
            set
            {
                _method = value;
            }
        }
        private int _maxTry = 100;
        public int maxTry
        {
            get { return _maxTry; }
            set
            {
                _maxTry = value;
            }
        }
    }
}

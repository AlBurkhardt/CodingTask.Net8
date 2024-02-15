using System.Net;

namespace CodingTaskServer
{
    public class Worker : BackgroundService
    {
        private readonly Dictionary<int, string> NumberToText;
        private readonly ILogger<Worker> Logger;
        private readonly string Host = "http://localhost:";
        private readonly string Port = "8001";

        public Worker(ILogger<Worker> logger)
        {
            Logger = logger;
            NumberToText = new Dictionary<int, string>
            {
                { 0, "zero" },
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
                { 4, "four" },
                { 5, "five" },
                { 6, "six" },
                { 7, "seven" },
                { 8, "eight" },
                { 9, "nine" },
                { 10, "ten" },
                { 11, "eleven" },
                { 12, "twelve" },
                { 13, "thirteen" },
                { 14, "fourteen" },
                { 15, "fifteen" },
                { 16, "sixteen" },
                { 17, "seventeen" },
                { 18, "eighteen" },
                { 19, "nineteen" },
                { 20, "twenty" },
                { 30, "thirty" },
                { 40, "forty" },
                { 50, "fifty" },
                { 60, "sixty" },
                { 70, "seventy" },
                { 80, "eighty" },
                { 90, "ninety" }
            };

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using HttpListener _listener = new();
            _listener.Prefixes.Add(Host + Port + "/");
            if (true)
            {

                _listener.Start();

                Logger.LogInformation(message: "Listening on port {port}...", Port);

                while (!stoppingToken.IsCancellationRequested)
                {
                    string _processed;
                    HttpListenerContext _context = _listener.GetContext();
                    using HttpListenerResponse _response = _context.Response;
                    using (StreamReader _reader = new(_context.Request.InputStream, _context.Request.ContentEncoding))
                    {
                        string _body = _reader.ReadToEnd();
                        _body = _body.Replace(" ", "");
                        _processed = ConvertData(_body);

                        Logger.LogInformation(message: "Request received with content: {body} converted to: {processed}", _body, _processed);
                    }

                    _response.StatusCode = (int)HttpStatusCode.OK;

                    string _responseString = _processed;
                    byte[] _buffer = System.Text.Encoding.UTF8.GetBytes(_responseString);
                    _response.ContentLength64 = _buffer.Length;

                    System.IO.Stream _outputStream = _response.OutputStream;
                    _outputStream.Write(_buffer, 0, _buffer.Length);
                    _outputStream.Close();
                }
            }

            while (stoppingToken.IsCancellationRequested)
            {
                _listener.Stop();
                Logger.LogInformation("Stop listening.");
                await Task.Delay(1000, stoppingToken);
            }
        }

        /// <summary>
        /// transform string with numbers to numbertext
        /// </summary>
        /// <param name="data">string of numbers</param>
        /// <returns>numbertext</returns>
        private string ConvertData(string data)
        {

            string _result = "";
            if (String.IsNullOrWhiteSpace(data))
            {
                return "no Input";
            }

            if (Double.TryParse(data, out _))
            {
                string _cents = "";
                bool _negativ = data[0] == '-';
                string _nonNegative = _negativ ? data[1..] : data;

                int _divIndex = _nonNegative.IndexOf(',');
                string _dollars;
                if (_divIndex == -1)
                {
                    _dollars = _nonNegative;
                }
                else
                {
                    if (_divIndex <= 0)
                    {
                        return "invalid input";
                    }
                    else
                    {
                        _dollars = _nonNegative[.._divIndex];
                        _cents = _nonNegative[(_divIndex + 1)..];
                    }

                }

                if (_negativ)
                {
                    _result = "minus ";
                }

                if (_dollars.Length > 9)
                {
                    return "input too large";
                }
                else
                {
                    if (Int32.TryParse(_dollars, out int _dlr))
                    {
                        if (_dlr == 1)
                        {
                            _result += "one dollar ";
                        }
                        else
                        {
                            if (_dlr == 0 && false)
                            {
                                _result += "zero dollars ";
                            }
                            else
                            {
                                _result += NumberTransform(_dollars) + "dollars ";
                            }

                        }
                    }
                }

                if (_cents.Length > 0)
                {
                    if (_cents.Length > 2)
                    {
                        _cents = _cents[..2];
                    }
                    else
                    {
                        if (_cents.Length == 1)
                        {
                            _cents += "0";
                        }
                    }
                    if (Int32.TryParse(_cents, out int _cts))
                    {
                        _result += "and ";
                        if (_cts == 1)
                        {
                            _result += "one cent";
                        }
                        else
                        {
                            _result += NumberTransform(_cents) + "cents";
                        }
                    }

                }

            }
            else
            {
                return "invalid input";
            }


            return _result;
        }

        /// <summary>
        /// convert string composed of digits to numbertext
        /// </summary>
        /// <param name="data">string with max nine digit</param>
        /// <returns>numbertext</returns>
        private string NumberTransform(string data)
        {
            string _result = "";
            string _rnData = data;
            if (data.Length == 0 || data.Length > 9)
            {
                return "";
            }
            if (_rnData.Length > 6)
            {
                int _milIndex = _rnData.Length - 6;
                string _milStrg = _rnData[.._milIndex];
                _rnData = _rnData[_milIndex..];
                _result += HundredToText(_milStrg) + "million ";

            }
            if (_rnData.Length > 3)
            {
                int _tosIndex = _rnData.Length - 3;
                string _tosStrg = _rnData[.._tosIndex];
                _rnData = _rnData[_tosIndex..];
                _result += HundredToText(_tosStrg) + "thousand ";
            }
            _result += HundredToText(_rnData);


            return _result;
        }

        /// <summary>
        /// convert string with max three digit to a numbertext
        /// </summary>
        /// <param name="data">string with max three digit</param>
        /// <returns>numbertext</returns>
        private string HundredToText(string data)
        {
            string _result = "";
            string _rnData = data;
            if (data == null || data.Length == 0 || data.Length > 3)
            {
                return "";
            }

            if (data.Length == 3)
            {
                if (Int32.TryParse(data[..1], out int _hrd))
                {
                    NumberToText.TryGetValue(_hrd, out string? _hrdStr);
                    if (_hrdStr != null)
                    {
                        _result += _hrdStr + " hundred ";
                        _rnData = _rnData[1..];
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }

            if (_rnData[0] == '0')
            {
                if (_rnData.Length > 1)
                {
                    if (Int32.TryParse(_rnData[1..], out int _rest))
                    {
                        if (_rest > 0)
                        {
                            NumberToText.TryGetValue(_rest, out string? _rstStr);
                            _result += _rstStr + " ";
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "zero ";
                }
            }
            else
            {
                if (Int32.TryParse(_rnData, out int _tens))
                {
                    if (_tens <= 20)
                    {
                        NumberToText.TryGetValue(_tens, out string? _tnsStr);
                        if (_tnsStr != null)
                        {
                            _result += _tnsStr + " ";
                        }
                        else
                        {
                            return "";
                        }
                    }
                    else
                    {
                        if (_rnData[1] == '0')
                        {
                            NumberToText.TryGetValue(_tens, out string? _tnsStr);
                            if (_tnsStr != null)
                            {
                                _result = _tnsStr + " ";
                            }
                            else
                            {
                                return "";
                            }
                        }
                        if (Int32.TryParse(_rnData[1..], out int _rest))
                        {
                            int _ten = _tens - _rest;
                            NumberToText.TryGetValue(_rest, out string? _rstStr);
                            NumberToText.TryGetValue(_ten, out string? _tnsStr);
                            if (_rstStr != null && _tnsStr != null)
                            {
                                _result += _tnsStr + "-" + _rstStr + " ";
                            }
                            else
                            {
                                return "";
                            }
                        }
                    }
                }
                else
                {
                    return "";
                }
            }


            return _result;
        }

    }
}

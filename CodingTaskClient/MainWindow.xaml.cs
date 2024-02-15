using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodingTaskClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CultureInfo CI { get; set; } = new CultureInfo("de-DE");
        private HttpClient Client { get; set; }

        private readonly string Host = "http://localhost:";
        private readonly string Port = "8001";
        public MainWindow()
        {
            InitializeComponent();
            Client = new HttpClient
            {
                BaseAddress = new Uri(Host + Port + "/")
            };

        }

        private void OnClickConvert(object sender, EventArgs e)
        {
            string _input = InputTB.Text;
            double? _dInput = StringToNDouble(_input);
            if (_dInput != null)
            {
                string _requestString = MaxPrecision((double)_dInput);
                try
                {
                    HttpContent _response = SendConvertRequest(_requestString);
                    using (StreamReader _reader = new(_response.ReadAsStream()))
                    {
                        string _body = _reader.ReadToEnd();
                        ResultTB.Text = _body;
                    }
                }
                catch
                {
                    ResultTB.Text = "Could not connect to server.";
                }
            }
        }

        /// <summary>
        ///  Wandelt den Double-Wert in einen String mit maximal 2 Nachkommastellen um. 
        /// </summary>
        /// <param name="input">Zu konvertierender Double-Wert</param>
        /// <returns>String Representation des Double-Wertes mit maximal 2 Nachkommasstellen</returns>
        private static string MaxPrecision(double input)
        {
            string _result = "";
            string _strInput = input.ToString();
            bool _symbolFound = false;
            short _nkS = 0;

            if (_strInput.Length > 0)
            {
                for (int _stringIndex = 0; _stringIndex < _strInput.Length; _stringIndex++)
                {
                    char _chr = _strInput[_stringIndex];
                    if (!_symbolFound)
                    {
                        _result += _chr;
                        if (_chr == ',')
                        {
                            _symbolFound = true;
                        }
                    }
                    else
                    {
                        if (_nkS < 2)
                        {
                            _nkS++;
                            _result += _chr;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return _result;
        }

        /// <summary>
        ///  Entfernt alle nicht Ziffern(ausser einem voranstehenden -),  alle . oder , (ausser dem erste zwischen Ziffern stehenden) und voranstehenden Nullen und wandelt den
        ///  String in einen Double-Wert um.
        /// </summary>
        /// <param name="double_IN">Zu konvertierender String</param>
        /// <returns>Konvertierte Double falls nicht konvertierbar dann Null</returns>
        private double? StringToNDouble(string double_IN)
        {
            double? _result = null;
            bool _isZero = false;
            bool _isDezimal = false;

            if (!String.IsNullOrWhiteSpace(double_IN))
            {
                string _cleanString = "";
                for (int _stringIndex = 0; _stringIndex < double_IN.Length; _stringIndex++)
                {
                    char _chr = double_IN[_stringIndex];
                    switch (_chr)
                    {
                        case '1':
                            _cleanString += _chr;
                            break;

                        case '2':
                            _cleanString += _chr;
                            break;

                        case '3':
                            _cleanString += _chr;
                            break;

                        case '4':
                            _cleanString += _chr;
                            break;

                        case '5':
                            _cleanString += _chr;
                            break;

                        case '6':
                            _cleanString += _chr;
                            break;

                        case '7':
                            _cleanString += _chr;
                            break;

                        case '8':
                            _cleanString += _chr;
                            break;

                        case '9':
                            _cleanString += _chr;
                            break;

                        case '0':
                            if (_cleanString.Length > 0 && _cleanString != "-")
                            {
                                _cleanString += _chr;
                            }
                            else
                            {
                                _isZero = true;
                            }
                            break;

                        case '-':
                            if (_cleanString.Length == 0 && !_isZero)
                            {
                                _cleanString += _chr;
                            }
                            break;

                        case ',':
                            if (!_isDezimal)
                            {
                                if (_cleanString.Length > 0 && _cleanString != "-")
                                {
                                    _cleanString += ',';
                                    _isDezimal = true;
                                }
                                else
                                {
                                    if (_isZero)
                                    {
                                        _cleanString += "0,";
                                        _isDezimal = true;
                                    }
                                }
                            }
                            break;

                        case '.':
                            if (!_isDezimal)
                            {
                                if (_cleanString.Length > 0 && _cleanString != "-")
                                {
                                    _cleanString += ',';
                                    _isDezimal = true;
                                }
                                else
                                {
                                    if (_isZero)
                                    {
                                        _cleanString += "0,";
                                        _isDezimal = true;
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
                if (_cleanString.Length > 0 && _cleanString[^1] == ',')
                {
                    _cleanString = _cleanString[..^1];
                }
                if (_cleanString.Length > 0 && _cleanString != "-")
                {
                    try
                    {
                        _result = double.Parse(_cleanString, CI);
                        return _result;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            if (_isZero)
            {
                return 0;
            }
            return _result;
        }

        /// <summary>
        ///  Sendet Convertierungs Anfrage an Server
        /// </summary>
        /// <param name="ConvertText">Zu konvertierender String</param>
        /// <returns>HttpContent der Response</returns>
        private HttpContent SendConvertRequest(string ConvertText)
        {
            HttpRequestMessage _httpRequest = new(HttpMethod.Post, "")
            {
                Content = new StringContent(ConvertText)
            };
            HttpResponseMessage _httpResponse = Client.Send(_httpRequest);
            return _httpResponse.Content;
        }
    }
}
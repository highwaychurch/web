using System;

namespace Highway.Shared.Mvc.FlashMessages
{
    [Serializable]
    public class FlashMessage
    {
        readonly FlashMessageDisposition _disposition;
        readonly string _heading;
        readonly string _message;

        public FlashMessage(FlashMessageDisposition disposition, string heading, string message)
        {
            _disposition = disposition;
            _heading = heading;
            _message = message;
        }

        public FlashMessage(FlashMessageDisposition disposition, string message)
        {
            _disposition = disposition;
            _message = message;
        }

        public FlashMessageDisposition Disposition
        {
            get { return _disposition; }
        }

        public string Heading
        {
            get { return _heading; }
        }

        public bool HasHeading
        {
            get { return string.IsNullOrWhiteSpace(_heading) == false; }
        }

        public string Message
        {
            get { return _message; }
        }

        public bool HasMessage
        {
            get { return string.IsNullOrWhiteSpace(_message) == false; }
        }

        public override string ToString()
        {
            return Disposition + ": " +
                (HasHeading ? Heading + " - " : "<no-heading> - ") +
                (HasMessage ? Message : "<no-message>");
        }
    }
}
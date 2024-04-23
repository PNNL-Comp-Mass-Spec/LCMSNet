using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortDevices.NumatoUsbRelays
{
    public class NumatoUsbRelayDevice
    {
        // TODO: Should probably use a static factory that will return common references to a single serial port connection, maybe?

        /*********
         * Command set:
         * 'ver': get current firmware version
         * 'id get': get programmed id
         * 'id set xxxxxxxx': set new programmed id, must be 8 alphanumeric (including symbols) characters
         * 'relay on x': turn relay on, relays appear to be numbered (across the different models) first 0-9, then A-F (16-relay) or A-V (32-relay). 64-relay module uses 00-63
         * 'relay off x': turn relay off
         * 'relay read x': read the current state of a relay, returns 'on' or 'off'
         * 'relay readall': returns a hex value (up to an integer in size) where a bitwise '0' means off and '1' means on, with relays numbered from right to left, so 0x00000001 would mean only relay 1 is on, 0x00000002 would mean only relay 2, etc. MAY NOT BE SUPPORTED on all lower-relay-count modules (shows up for x-channel USB Powered Relay Module, but not for 2/4-channel USB Relay Module)
         * 'relay writeall xxxxxxxx': set the value of the relays according to the above; number of hex characters should correspond to the number of relays. MAY NOT BE SUPPORTED on all lower-relay-count modules (shows up for x-channel USB Powered Relay Module, but not for 2/4-channel USB Relay Module)
         * 'gpio set x': set the GPIO output status to 'high', 'x' is the GPIO number
         * 'gpio clear x': set the GPIO output status to 'low'
         * 'gpio read x': read the digital high/low status at the input, returns 'on' or 'off'
         * * more gpio commands available for some of the other USB devices Numato Lab's produces (the USB GPIO devices)
         * 'adc read x': reads the analog voltage at the specified input, returns 0-1023, a 10-bit resolution; '0' is 0V, 1023 is either 3.3V or 5V (depends on the module)
         */
    }
}

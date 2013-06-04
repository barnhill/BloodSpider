#!/usr/bin/perl

use vars qw($OS_win);

#boilerplate serial port setup stuff, needed for any serial script

BEGIN {
        $OS_win = ($^O eq "MSWin32") ? 1 : 0;

        print "Perl version: $]\n";
        print "OS   version: $^O\n";

            # This must be in a BEGIN in order for the 'use' to be conditional
        if ($OS_win) {
            print "Loading Windows modules\n";
            eval "use Win32::SerialPort";
	    die "$@\n" if ($@);

        }
        else {
            print "Loading Unix modules\n";
            eval "use Device::SerialPort";
	    die "$@\n" if ($@);
        }
}                               # End BEGIN

#Right now I specify the COM port as the first command line argument.
#die "\n\nno port specified\n" unless ($ARGV[0]);
#my $port = $ARGV[0];
my $port = "/dev/ttyS1";

my $meterport; 

#More boilerplate stuff
if ($OS_win) {
    $meterport = new Win32::SerialPort ($port,1);
}
else {
    $meterport = new Device::SerialPort ($port,1);
}
die "Can't open serial port $port: $^E\n" unless ($meterport);

#Not necessary if you only want to sleep() for integer amounts of seconds.
use Time::HiRes qw ( sleep );

#more boilerplate stuff
$meterport->user_msg(1);	# misc. warnings
$meterport->error_msg(1);	# hardware and data errors

#Set up the IR port.  9600 N,8,1  Change as needed
$meterport->devicetype('none');
$meterport->handshake('none');
$meterport->baudrate(9600);
$meterport->parity('none');
$meterport->databits(8);
$meterport->stopbits(1);
$meterport->write_settings;

#die "\n\nNo output file specified\n" unless ($ARGV[0]);
#$fname = $ARGV[0];

#Clear contents of file
#open LOGFILE, ">" . $fname or die "Couldn't open output file ".$fname.": $!\n";
#close LOGFILE;

$two = chr(2);
$three = chr(3);
$four = chr(4);
$six = chr(6);

#Clear serial buffer - I do this by writing a command that changes nothing, then reading back all responses and discarding them.
$out = pack(C,1);
$foo = $meterport->write($out);
sleep 0.1;
$bah = $meterport->input;
sleep 0.1;

$out = pack(C,hex(D));
$foo = $meterport->write($out);
sleep 0.1;

$out = pack(C,hex(B));
$foo = $meterport->write($out);
sleep 0.1;
$foo = $meterport->input;
$out = pack(C,hex(D));
$foo = $meterport->write($out);
sleep 0.2;
$bah = $meterport->input;
$bah =~ s/$six//g;
$bah =~ s/$four//g;
$bah =~ s/$two//g;
#print "0x0B\n" . $bah . "\n";


$out = "C";
$foo = $meterport->write($out);
sleep 0.1;
$out = "4";
$foo = $meterport->write($out);
sleep 0.1;
$foo = $meterport->input;
$out = pack("C",hex(D));
$foo = $meterport->write($out);
sleep 0.2;
$bah = $meterport->input;
$bah =~ s/$six//g;
$bah =~ s/$four//g;
$bah =~ s/$two//g;
#print "C4\n" . $bah . "\n";



$out = "C";
$foo = $meterport->write($out);
sleep 0.1;
$out = " ";
$foo = $meterport->write($out);
sleep 0.1;
$out = "3";
$foo = $meterport->write($out);
sleep 0.1;
$bah = $meterport->input;

$out = pack("C",hex(D));
$foo = $meterport->write($out);
sleep 0.5;
$bah = $meterport->input;
$bah =~ s/$six//g;
$bah =~ s/$four//g;
$bah =~ s/$two//g;
#print "C 3\n" . $bah . "\n";


$out = "S";
$foo = $meterport->write($out);
sleep 0.1;
$out = " ";
$foo = $meterport->write($out);
sleep 0.1;
$bah = $meterport->input;
$out = "1";
$foo = $meterport->write($out);
sleep 0.1;
$bah = $meterport->input;

$out = pack("C",hex(D));
$foo = $meterport->write($out);
sleep 0.2;
$bah = $meterport->input;
$bah =~ s/$six//g;
$bah =~ s/$four//g;
$bah =~ s/$two//g;
#print "S 1\n" . $bah . "\n";


$out = "S";
$foo = $meterport->write($out);
sleep 0.1;
$out = " ";
$foo = $meterport->write($out);
sleep 0.1;
$out = "2";
$foo = $meterport->write($out);
sleep 0.1;
$bah = $meterport->input;

$out = pack("C",hex(D));
$foo = $meterport->write($out);
sleep 0.2;
$bah = $meterport->input;
$bah =~ s/$six//g;
$bah =~ s/$four//g;
$bah =~ s/$two//g;
#print "S 2\n". $bah . "\n";

$out = "`";
$foo = $meterport->write($out);
sleep 0.1;
$bah = $meterport->input;

$out = pack("C",hex(D));
$foo = $meterport->write($out);
sleep 0.2;
$bah = $meterport->input;
$bah =~ s/$six//g;
$bah =~ s/$four//g;
$bah =~ s/$two//g;
$numreadings = substr($bah,3,3)*1;
#print "`\n" . $bah . "\n";


$out = "S";
$foo = $meterport->write($out);
sleep 0.1;
$out = " ";
$foo = $meterport->write($out);
sleep 0.1;
$out = "3";
$foo = $meterport->write($out);
sleep 0.1;
$bah = $meterport->input;

$out = pack("C",hex(D));
$foo = $meterport->write($out);
sleep 0.2;
$bah = $meterport->input;
$bah =~ s/$six//g;
$bah =~ s/$four//g;
$bah =~ s/$two//g;
#print "S 3\n". $bah . "\n";


$out = "a";
$foo = $meterport->write($out);
sleep 0.1;
$out = " ";
$foo = $meterport->write($out);
sleep 0.1;
$out = "1";
$foo = $meterport->write($out);
sleep 0.1;
$out = " ";
$foo = $meterport->write($out);
sleep 0.1;
for($i = 0; $i < length($numreadings); $i++)
  {
    $out = substr($numreadings,$i,1);
    $foo = $meterport->write($out);
    sleep 0.1;
  }
$bah = $meterport->input;


$out = pack("C",hex(D));
$foo = $meterport->write($out);
sleep 0.3;
$bah = $meterport->input;
$bah =~ s/$six//g;
$bah =~ s/$four//g;
$bah =~ s/$three//g;
$bah =~ s/$two//g;
#print "a 1 5\n". $bah . "\n";
$reading = substr($bah,3,3)*1;
$hour = substr($bah,6,2);
$minute = substr($bah,8,2);
$day = substr($bah,10,2);
$month = substr($bah,12,2);
$year = substr($bah,14,2);
print "$month/$day/20$year $hour:$minute - $reading\n";
for ($i = 2; $i <= $numreadings; $i++)
  {
    $out = chr(6);
    $foo = $meterport->write($out);
    sleep 0.3;
    $bah = $meterport->input;
    $bah =~ s/$four//g;
    $bah =~ s/$three//g;
    $bah =~ s/$two//g;
    #print $bah . "\n";
    $reading = substr($bah,3,3)*1;
    $hour = substr($bah,6,2);
    $minute = substr($bah,8,2);
    $day = substr($bah,10,2);
    $month = substr($bah,12,2);
    $year = substr($bah,14,2);
    print "$month/$day/20$year $hour:$minute - $reading\n";
  }


$out = pack("C",hex(0x1D));
$foo = $meterport->write($out);
sleep 0.1;
$bah = $meterport->input;
print $bah . "\n";

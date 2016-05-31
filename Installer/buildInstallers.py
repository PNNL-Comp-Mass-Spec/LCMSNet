import os
from   datetime import date
import sys
import subprocess
import shutil

path64Bit = "C:\Program Files (x86)\Inno Setup 5\Compil32.exe"
path32Bit = "C:\Program Files\Inno Setup 5\Compil32.exe"
final     = "\\\\proto-5\\bionetXfer\\Installers\\Software\\LCMSNet"

if (len(sys.argv) < 1):
    print "usage: buildInstallers.py installerPath" 
    exit(1)

ename = os.path.join(sys.argv[1], "LCMSNet_X86-external.iss")
pname = os.path.join(sys.argv[1], "LCMSNet_X86-pnnl.iss")

externalCommand = "/cc %s" % (ename) 
pnnlCommand     = "/cc %s" % (pname)
ecommand        = ""
pcommand        = ""

if (os.path.exists(path64Bit)):
    ecommand =  "%s %s" % (path64Bit, externalCommand)
    pcommand =  "%s %s" % (path64Bit, pnnlCommand)
elif (os.path.exists(path32Bit)):    
    ecommand =  "%s %s" % (path32Bit, externalCommand)
    pcommand =  "%s %s" % (path32Bit, pnnlCommand)
else:
    print "The installer package was not found at either location:"
    print "\t", path64Bit
    print "\t", path32Bit
    exit(1)

print "Building external app: ", ecommand
#pe = subprocess.Popen(ecommand)
#pe.wait()
print "Building internal app: ", pcommand
#pp = subprocess.Popen(pcommand)
#pp.wait()

os.chdir(sys.argv[1])

p = "..\\..\\LCMSNetInstaller"
d = date.today()
x = d.strftime("%m_%d_%y")
p = p + "\\" + x
p = os.path.abspath(p)
print p
installers = os.listdir(os.path.abspath(p))

for installer in installers:
    pi = os.path.join(p, installer)
    print pi
    raw_input(":")
    shutil.copy(pi, final)

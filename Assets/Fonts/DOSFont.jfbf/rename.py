from os import system
for i in range(0,256):
	h = "%08X.png"%i
	d = "%d.png"%i
	print(h," => ",d)
	system("ren %s %s"%(h,d))

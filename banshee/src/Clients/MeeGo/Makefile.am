ASSEMBLY = MeeGo
TARGET = exe
LINK = $(REF_MEEGO)
SOURCES = Banshee.MeeGo.Client/MeeGoClient.cs 

if HAVE_MEEGO
include $(top_srcdir)/build/build.mk
else
include $(top_srcdir)/build/build.dist.mk
endif

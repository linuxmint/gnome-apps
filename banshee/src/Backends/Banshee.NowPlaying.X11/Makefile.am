SUBDIRS = libbnpx11

ASSEMBLY = Banshee.NowPlaying.X11
TARGET = library
LINK = $(REF_BACKEND_BNPX11)
INSTALL_DIR = $(BACKENDS_INSTALL_DIR)

SOURCES = Banshee.NowPlaying.X11/FullscreenAdapter.cs

RESOURCES = Banshee.NowPlaying.X11.addin.xml

if HAVE_XVIDMODE
include $(top_srcdir)/build/build.mk
dist_module_SCRIPTS = Banshee.NowPlaying.X11.dll.config
else
include $(top_srcdir)/build/build.dist.mk
endif


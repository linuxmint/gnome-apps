pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_in_files = $(wildcard banshee-*.pc.in)
pkgconfig_DATA = $(pkgconfig_in_files:.pc.in=.pc)

%.pc: %.pc.in $(top_builddir)/configure.ac
	sed "s,\@VERSION\@,$(VERSION),g; s,\@prefix\@,$(prefix),g; s,\@libdir\@,$(libdir),g; s,\@PACKAGE\@,$(PACKAGE),g" < $< > $@

EXTRA_DIST = $(pkgconfig_in_files)

CLEANFILES = $(pkgconfig_DATA)
MAINTAINERCLEANFILES = Makefile.in


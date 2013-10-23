SUBDIRS = tests

INCLUDES = \
	-DG_LOG_DOMAIN=\"gnome-system-log\"			\
	-DGNOMELOCALEDIR=\""$(prefix)/$(DATADIRNAME)/locale"\"	\
	-DDATADIR=\""$(datadir)"\"				\
	-DLOGVIEWINSTALLPREFIX=\""$(prefix)/\""			\
	-DLOGVIEW_DATADIR=\""$(pkgdatadir)"\"			\
	$(SUN_OS)						\
	$(NULL)

bin_PROGRAMS = gnome-system-log

BUILT_SOURCES = 		\
	logview-marshal.c	\
	logview-marshal.h

gnome_system_log_SOURCES = 	\
	logview-app.c		\
	logview-app.h		\
	logview-main.c		\
	logview-about.h		\
	logview-manager.c	\
	logview-manager.h	\
	logview-utils.c		\
	logview-utils.h		\
	logview-loglist.c	\
	logview-loglist.h	\
	logview-window.c	\
	logview-window.h	\
	logview-log.h		\
	logview-log.c		\
	logview-findbar.h	\
	logview-findbar.c	\
	logview-prefs.c		\
	logview-prefs.h		\
	logview-filter.h       \
	logview-filter.c       \
	logview-filter-manager.h       \
	logview-filter-manager.c  \
	$(BUILT_SOURCES)

gnome_system_log_CFLAGS =	\
	$(LOGVIEW_CFLAGS)		\
	$(NULL)

gnome_system_log_LDADD =	\
	$(LOGVIEW_LIBS)		\
	$(Z_LIBS)		\
	-lm

logview-marshal.h: logview-marshal.list $(GLIB_GENMARSHAL)
	$(GLIB_GENMARSHAL) $< --header --prefix=logview_marshal >> $@

logview-marshal.c: logview-marshal.list $(GLIB_GENMARSHAL)
	echo "#include \"logview-marshal.h\"" > $@ && \
	$(GLIB_GENMARSHAL) $< --body --prefix=logview_marshal >> $@

EXTRA_DIST = logview-marshal.list

CLEANFILES = 				\
	$(BUILT_SOURCES)

dist-hook:
	cd $(distdir) ; rm -f $(CLEANFILES)


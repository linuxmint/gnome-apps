INCLUDES = \
	-I../

noinst_PROGRAMS = test-reader

test_reader_SOURCES = \
	test-reader.c		\
	../logview-log.c	\
	../logview-utils.c

test_reader_CFLAGS = \
	$(LOGVIEW_CFLAGS)	\
	$(NULL)

test_reader_LDADD = \
	$(LOGVIEW_LIBS)		\
	$(Z_LIBS)		\
	-lm

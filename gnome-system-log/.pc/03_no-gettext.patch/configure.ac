dnl -*- mode: m4 -*-
AC_PREREQ(2.63)
AC_INIT([gnome-system-log],[3.4.1],[],[gnome-system-log])

AC_CONFIG_HEADERS(config.h)
AC_CONFIG_MACRO_DIR([m4])

m4_ifdef([AM_SILENT_RULES],[AM_SILENT_RULES([yes])])

AC_CANONICAL_TARGET

AM_INIT_AUTOMAKE([1.11 dist-xz no-dist-gzip foreign])
AM_SILENT_RULES([yes])
AM_MAINTAINER_MODE

GETTEXT_PACKAGE=AC_PACKAGE_NAME
AC_SUBST(GETTEXT_PACKAGE)
AC_DEFINE_UNQUOTED(GETTEXT_PACKAGE,"$GETTEXT_PACKAGE",[The name of the gettext domain])
IT_PROG_INTLTOOL(0.40.0)
PKG_PROG_PKG_CONFIG([0.22])

AC_PATH_PROG(GLIB_GENMARSHAL, [glib-genmarshal])

AM_GNU_GETTEXT([external])
AM_GNU_GETTEXT_VERSION([0.17])

AC_PROG_CC
AM_PROG_CC_C_O
AC_PROG_CXX
AC_ISC_POSIX
AC_HEADER_STDC

## don't rerun to this point if we abort
AC_CACHE_SAVE

GLIB_GSETTINGS

GNOME_DOC_INIT

AC_ARG_ENABLE([zlib],
              [AS_HELP_STRING([--disable-zlib],[disable zlib support])])
msg_zlib=no
Z_LIBS=

AS_IF([test "x$enable_zlib" != "xno"],
      [
        AC_CHECK_HEADER([zlib.h], [AC_CHECK_LIB([z], [inflate], [msg_zlib=yes])])

        AS_IF([test "x$msg_zlib" = "xyes"],
              [
                AC_DEFINE(HAVE_ZLIB, [1],
                          [Define to 1 if we're building with ZLib support])
                Z_LIBS="-lz"
              ]
        )
      ]
)

AC_SUBST(Z_LIBS)

GLIB_MIN_VERSION=2.31.0
GTK_MIN_VERSION=3.0.0

PKG_CHECK_MODULES(LOGVIEW,
                  glib-2.0 >= $GLIB_MIN_VERSION
                  gtk+-3.0 >= $GTK_MIN_VERSION)

AC_CONFIG_FILES([
Makefile
data/Makefile
data/icons/Makefile
help/Makefile
src/Makefile
src/tests/Makefile
po/Makefile.in
])

AC_OUTPUT

dnl ==========================================================================
echo "

        GNOME Search Tool $VERSION
        ==========================

        prefix:    ${prefix}
        compiler:  ${CC}

        zlib support: ${msg_zlib}

        Now type 'make' to build $PACKAGE
"

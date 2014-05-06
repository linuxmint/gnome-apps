AC_DEFUN([BANSHEE_CHECK_MONO_UPNP],
[
	MONOUPNP_REQUIRED=0.1

	AC_ARG_ENABLE([upnp],
		AC_HELP_STRING([--enable-upnp], [Enable UPnP support]),
		enable_upnp=$enableval, enable_upnp="try"
	)

	has_mono_upnp=no
	if test "x$enable_upnp" != "xno"; then
		PKG_CHECK_MODULES(MONO_UPNP,
			mono.ssdp >= $MONOUPNP_REQUIRED
			mono.upnp >= $MONOUPNP_REQUIRED
			mono.upnp.dcp.mediaserver1 >= $MONOUPNP_REQUIRED,
			has_mono_upnp=yes, has_mono_upnp=no)
	fi

	if test "x$enable_upnp" = "xyes" -a "x$has_mono_upnp" = "xno"; then
		AC_MSG_ERROR([mono-upnp was not found or is not up to date. Please install mono-upnp of at least version $MONOUPNP_REQUIRED, or disable UPnP support by passing --disable-upnp])
	fi

	if test "x$enable_upnp" = "xtry" -a "x$has_mono_upnp" = "xyes"; then
		enable_upnp=yes
	fi

	if test "x$enable_upnp" = "xyes"; then
		AC_SUBST(MONO_UPNP_LIBS)

		asms="`$PKG_CONFIG --variable=Libraries mono.ssdp` `$PKG_CONFIG --variable=Libraries mono.upnp` `$PKG_CONFIG --variable=Libraries mono.upnp.dcp.mediaserver1`"
		for asm in $asms; do
			FILENAME=`basename $asm`
			if [[ "`echo $SEENBEFORE | grep $FILENAME`" = "" ]]; then
				MONOUPNP_ASSEMBLIES="$MONOUPNP_ASSEMBLIES $asm"
				[[ -r "$asm.config" ]] && MONOUPNP_ASSEMBLIES="$MONOUPNP_ASSEMBLIES $asm.config"
				[[ -r "$asm.mdb" ]] && MONOUPNP_ASSEMBLIES="$MONOUPNP_ASSEMBLIES $asm.mdb"
				SEENBEFORE="$SEENBEFORE $FILENAME"
			fi
		done
		AC_SUBST(MONOUPNP_ASSEMBLIES)

		AM_CONDITIONAL(UPNP_ENABLED, true)
	else
		enable_upnp=no
		AM_CONDITIONAL(UPNP_ENABLED, false)
	fi

])

-----BEGIN PGP SIGNED MESSAGE-----
Hash: SHA1

Format: 3.0 (quilt)
Source: gtk+3.0
Binary: libgtk-3-0, libgtk-3-0-udeb, libgtk-3-common, libgtk-3-bin, libgtk-3-dev, libgtk-3-0-dbg, libgtk-3-doc, gtk-3-examples, gir1.2-gtk-3.0, libgail-3-0, libgail-3-dev, libgail-3-0-dbg, libgail-3-doc
Architecture: any all
Version: 3.8.6-0ubuntu2
Maintainer: Ubuntu Developers <ubuntu-devel-discuss@lists.ubuntu.com>
Uploaders: Debian GNOME Maintainers <pkg-gnome-maintainers@lists.alioth.debian.org>, Jeremy Bicha <jbicha@ubuntu.com>
Homepage: http://www.gtk.org/
Standards-Version: 3.9.3
Vcs-Bzr: https://code.launchpad.net/~ubuntu-desktop/gtk/ubuntugtk3
Build-Depends: debhelper (>= 8.1.3), cdbs (>= 0.4.93~), gnome-pkg-tools (>= 0.11), dpkg-dev (>= 1.16.0), gtk-doc-tools (>= 1.11), dh-autoreconf, gettext, pkg-config, autotools-dev, libglib2.0-dev (>= 2.35.3), libgdk-pixbuf2.0-dev (>= 2.27.1), libpango1.0-dev (>= 1.32.4), libatk1.0-dev (>= 2.7.5), libatk-bridge2.0-dev, libx11-dev (>= 2:1.3.3-2), libxext-dev (>= 2:1.1.1-3), libxi-dev (>= 2:1.3-4), libxrandr-dev (>= 2:1.2.99), libxt-dev, libxrender-dev (>= 1:0.9.5-2), libxft-dev, libxcursor-dev (>= 1:1.1.10-2), libxcomposite-dev (>= 1:0.2.0-3), libxdamage-dev (>= 1:1.0.1-3), libxkbfile-dev, libxinerama-dev (>= 2:1.1-3), libxfixes-dev (>= 1:3.0.0-3), libcairo2-dev (>= 1.10.0), x11proto-xext-dev, libcups2-dev (>= 1.2), libcolord-dev (>= 0.1.9), gobject-introspection (>= 1.32.0), libgirepository1.0-dev (>= 1.32.0), xvfb, libwayland-dev (>= 1.0.0), libxkbcommon-dev (>= 0.2.0-0ubuntu3~)
Build-Depends-Indep: docbook-xml, docbook-utils, libglib2.0-doc, libatk1.0-doc, libpango1.0-doc, libcairo2-doc
Package-List: 
 gir1.2-gtk-3.0 deb introspection optional
 gtk-3-examples deb x11 extra
 libgail-3-0 deb libs optional
 libgail-3-0-dbg deb debug extra
 libgail-3-dev deb libdevel optional
 libgail-3-doc deb doc optional
 libgtk-3-0 deb libs optional
 libgtk-3-0-dbg deb debug extra
 libgtk-3-0-udeb udeb debian-installer extra
 libgtk-3-bin deb misc optional
 libgtk-3-common deb misc optional
 libgtk-3-dev deb libdevel optional
 libgtk-3-doc deb doc optional
Checksums-Sha1: 
 0fe62a9260595b2ed9ddf3b5dcb892014663e7f2 13854916 gtk+3.0_3.8.6.orig.tar.xz
 ae4048e792e3172a438c5299f52f798fd2be9126 122844 gtk+3.0_3.8.6-0ubuntu2.debian.tar.gz
Checksums-Sha256: 
 b5638b2d2ffa1b3aa2dbdba75a3134d9dc4b6e4ed7a287855fed2811956cd4ed 13854916 gtk+3.0_3.8.6.orig.tar.xz
 e0acab4c648cce91f43a92973bbbd195c1e796cf0d1875ee3c96eccbde7bbfee 122844 gtk+3.0_3.8.6-0ubuntu2.debian.tar.gz
Files: 
 8614fc55da423fe99cffcd4e66c1aaf3 13854916 gtk+3.0_3.8.6.orig.tar.xz
 ac5668b7fb1c211ee18c959405367f62 122844 gtk+3.0_3.8.6-0ubuntu2.debian.tar.gz
Original-Maintainer: Debian GNOME Maintainers <pkg-gnome-maintainers@lists.alioth.debian.org>
Testsuite: autopkgtest

-----BEGIN PGP SIGNATURE-----
Version: GnuPG v1.4.14 (GNU/Linux)

iEYEARECAAYFAlJupp4ACgkQQxo87aLX0pInMQCgxYkRtrQ+A22pJOuPHHrPjEnq
wo0AoOjUJx7EXQHBdTSa6pMLCSCRXLGX
=Fo9a
-----END PGP SIGNATURE-----

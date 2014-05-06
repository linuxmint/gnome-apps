//
// banshee-player-dvd.h
//
// Author:
//   Alex Launi <alex.launi@canonical.com>
//
// Copyright (C) 2010 Alex Launi
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#ifndef _BANSHEE_PLAYER_DVD_H
#define _BANSHEE_PLAYER_DVD_H

#include "banshee-player-private.h"

void      _bp_dvd_pipeline_setup            (BansheePlayer *player);
gboolean  _bp_dvd_handle_uri                (BansheePlayer *player, const gchar *uri);
void      _bp_dvd_find_navigation           (BansheePlayer *player);
void      _bp_dvd_elements_process_message  (BansheePlayer *player, GstMessage *message);

#endif /* _BANSHEE_PLAYER_DVD_H */

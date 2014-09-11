


#ifndef __LIBYELP_TYPES_H__
#define __LIBYELP_TYPES_H__

#include <glib-object.h>

G_BEGIN_DECLS

#include "yelp-document.h"
#define YELP_TYPE_DOCUMENT_SIGNAL yelp_document_signal_get_type()
GType yelp_document_signal_get_type (void);
#include "yelp-settings.h"
#define YELP_TYPE_SETTINGS_COLOR yelp_settings_color_get_type()
GType yelp_settings_color_get_type (void);
#define YELP_TYPE_SETTINGS_FONT yelp_settings_font_get_type()
GType yelp_settings_font_get_type (void);
#define YELP_TYPE_SETTINGS_ICON yelp_settings_icon_get_type()
GType yelp_settings_icon_get_type (void);
#include "yelp-uri.h"
#define YELP_TYPE_URI_DOCUMENT_TYPE yelp_uri_document_type_get_type()
GType yelp_uri_document_type_get_type (void);
#include "yelp-view.h"
#define YELP_TYPE_VIEW_STATE yelp_view_state_get_type()
GType yelp_view_state_get_type (void);
G_END_DECLS

#endif /* __LIBYELP_TYPES_H__ */




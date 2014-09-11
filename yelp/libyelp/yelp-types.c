


#include <glib-object.h>
#include "yelp-types.h"


/* enumerations from "yelp-document.h" */
static const GEnumValue _yelp_document_signal_values[] = {
  { YELP_DOCUMENT_SIGNAL_CONTENTS, "YELP_DOCUMENT_SIGNAL_CONTENTS", "contents" },
  { YELP_DOCUMENT_SIGNAL_INFO, "YELP_DOCUMENT_SIGNAL_INFO", "info" },
  { YELP_DOCUMENT_SIGNAL_ERROR, "YELP_DOCUMENT_SIGNAL_ERROR", "error" },
  { 0, NULL, NULL }
};

GType
yelp_document_signal_get_type (void)
{
  static GType type = 0;

  if (!type)
    type = g_enum_register_static ("YelpDocumentSignal", _yelp_document_signal_values);

  return type;
}


/* enumerations from "yelp-settings.h" */
static const GEnumValue _yelp_settings_color_values[] = {
  { YELP_SETTINGS_COLOR_BASE, "YELP_SETTINGS_COLOR_BASE", "color-base" },
  { YELP_SETTINGS_COLOR_TEXT, "YELP_SETTINGS_COLOR_TEXT", "color-text" },
  { YELP_SETTINGS_COLOR_TEXT_LIGHT, "YELP_SETTINGS_COLOR_TEXT_LIGHT", "color-text-light" },
  { YELP_SETTINGS_COLOR_LINK, "YELP_SETTINGS_COLOR_LINK", "color-link" },
  { YELP_SETTINGS_COLOR_LINK_VISITED, "YELP_SETTINGS_COLOR_LINK_VISITED", "color-link-visited" },
  { YELP_SETTINGS_COLOR_GRAY_BASE, "YELP_SETTINGS_COLOR_GRAY_BASE", "color-gray-base" },
  { YELP_SETTINGS_COLOR_DARK_BASE, "YELP_SETTINGS_COLOR_DARK_BASE", "color-dark-base" },
  { YELP_SETTINGS_COLOR_GRAY_BORDER, "YELP_SETTINGS_COLOR_GRAY_BORDER", "color-gray-border" },
  { YELP_SETTINGS_COLOR_BLUE_BASE, "YELP_SETTINGS_COLOR_BLUE_BASE", "color-blue-base" },
  { YELP_SETTINGS_COLOR_BLUE_BORDER, "YELP_SETTINGS_COLOR_BLUE_BORDER", "color-blue-border" },
  { YELP_SETTINGS_COLOR_RED_BASE, "YELP_SETTINGS_COLOR_RED_BASE", "color-red-base" },
  { YELP_SETTINGS_COLOR_RED_BORDER, "YELP_SETTINGS_COLOR_RED_BORDER", "color-red-border" },
  { YELP_SETTINGS_COLOR_YELLOW_BASE, "YELP_SETTINGS_COLOR_YELLOW_BASE", "color-yellow-base" },
  { YELP_SETTINGS_COLOR_YELLOW_BORDER, "YELP_SETTINGS_COLOR_YELLOW_BORDER", "color-yellow-border" },
  { YELP_SETTINGS_NUM_COLORS, "YELP_SETTINGS_NUM_COLORS", "num-colors" },
  { 0, NULL, NULL }
};

GType
yelp_settings_color_get_type (void)
{
  static GType type = 0;

  if (!type)
    type = g_enum_register_static ("YelpSettingsColor", _yelp_settings_color_values);

  return type;
}

static const GEnumValue _yelp_settings_font_values[] = {
  { YELP_SETTINGS_FONT_VARIABLE, "YELP_SETTINGS_FONT_VARIABLE", "font-variable" },
  { YELP_SETTINGS_FONT_FIXED, "YELP_SETTINGS_FONT_FIXED", "font-fixed" },
  { YELP_SETTINGS_NUM_FONTS, "YELP_SETTINGS_NUM_FONTS", "num-fonts" },
  { 0, NULL, NULL }
};

GType
yelp_settings_font_get_type (void)
{
  static GType type = 0;

  if (!type)
    type = g_enum_register_static ("YelpSettingsFont", _yelp_settings_font_values);

  return type;
}

static const GEnumValue _yelp_settings_icon_values[] = {
  { YELP_SETTINGS_ICON_BUG, "YELP_SETTINGS_ICON_BUG", "icon-bug" },
  { YELP_SETTINGS_ICON_IMPORTANT, "YELP_SETTINGS_ICON_IMPORTANT", "icon-important" },
  { YELP_SETTINGS_ICON_NOTE, "YELP_SETTINGS_ICON_NOTE", "icon-note" },
  { YELP_SETTINGS_ICON_TIP, "YELP_SETTINGS_ICON_TIP", "icon-tip" },
  { YELP_SETTINGS_ICON_WARNING, "YELP_SETTINGS_ICON_WARNING", "icon-warning" },
  { YELP_SETTINGS_NUM_ICONS, "YELP_SETTINGS_NUM_ICONS", "num-icons" },
  { 0, NULL, NULL }
};

GType
yelp_settings_icon_get_type (void)
{
  static GType type = 0;

  if (!type)
    type = g_enum_register_static ("YelpSettingsIcon", _yelp_settings_icon_values);

  return type;
}


/* enumerations from "yelp-uri.h" */
static const GEnumValue _yelp_uri_document_type_values[] = {
  { YELP_URI_DOCUMENT_TYPE_UNRESOLVED, "YELP_URI_DOCUMENT_TYPE_UNRESOLVED", "unresolved" },
  { YELP_URI_DOCUMENT_TYPE_DOCBOOK, "YELP_URI_DOCUMENT_TYPE_DOCBOOK", "docbook" },
  { YELP_URI_DOCUMENT_TYPE_MALLARD, "YELP_URI_DOCUMENT_TYPE_MALLARD", "mallard" },
  { YELP_URI_DOCUMENT_TYPE_MAN, "YELP_URI_DOCUMENT_TYPE_MAN", "man" },
  { YELP_URI_DOCUMENT_TYPE_INFO, "YELP_URI_DOCUMENT_TYPE_INFO", "info" },
  { YELP_URI_DOCUMENT_TYPE_TEXT, "YELP_URI_DOCUMENT_TYPE_TEXT", "text" },
  { YELP_URI_DOCUMENT_TYPE_HTML, "YELP_URI_DOCUMENT_TYPE_HTML", "html" },
  { YELP_URI_DOCUMENT_TYPE_XHTML, "YELP_URI_DOCUMENT_TYPE_XHTML", "xhtml" },
  { YELP_URI_DOCUMENT_TYPE_HELP_LIST, "YELP_URI_DOCUMENT_TYPE_HELP_LIST", "help-list" },
  { YELP_URI_DOCUMENT_TYPE_NOT_FOUND, "YELP_URI_DOCUMENT_TYPE_NOT_FOUND", "not-found" },
  { YELP_URI_DOCUMENT_TYPE_EXTERNAL, "YELP_URI_DOCUMENT_TYPE_EXTERNAL", "external" },
  { YELP_URI_DOCUMENT_TYPE_ERROR, "YELP_URI_DOCUMENT_TYPE_ERROR", "error" },
  { 0, NULL, NULL }
};

GType
yelp_uri_document_type_get_type (void)
{
  static GType type = 0;

  if (!type)
    type = g_enum_register_static ("YelpUriDocumentType", _yelp_uri_document_type_values);

  return type;
}


/* enumerations from "yelp-view.h" */
static const GEnumValue _yelp_view_state_values[] = {
  { YELP_VIEW_STATE_BLANK, "YELP_VIEW_STATE_BLANK", "blank" },
  { YELP_VIEW_STATE_LOADING, "YELP_VIEW_STATE_LOADING", "loading" },
  { YELP_VIEW_STATE_LOADED, "YELP_VIEW_STATE_LOADED", "loaded" },
  { YELP_VIEW_STATE_ERROR, "YELP_VIEW_STATE_ERROR", "error" },
  { 0, NULL, NULL }
};

GType
yelp_view_state_get_type (void)
{
  static GType type = 0;

  if (!type)
    type = g_enum_register_static ("YelpViewState", _yelp_view_state_values);

  return type;
}





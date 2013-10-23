
/* Generated data (by glib-mkenums) */

#include "gedit-file-browser-enum-types.h"

/* enumerations from "gedit-file-browser-store.h" */
#include "gedit-file-browser-store.h"

static GType gedit_file_browser_store_column_type = 0;

static GType
register_gedit_file_browser_store_column (GTypeModule *module)
{
	static const GEnumValue values[] = {
		{ GEDIT_FILE_BROWSER_STORE_COLUMN_ICON,
		  "GEDIT_FILE_BROWSER_STORE_COLUMN_ICON",
		  "icon" },
		{ GEDIT_FILE_BROWSER_STORE_COLUMN_NAME,
		  "GEDIT_FILE_BROWSER_STORE_COLUMN_NAME",
		  "name" },
		{ GEDIT_FILE_BROWSER_STORE_COLUMN_URI,
		  "GEDIT_FILE_BROWSER_STORE_COLUMN_URI",
		  "uri" },
		{ GEDIT_FILE_BROWSER_STORE_COLUMN_FLAGS,
		  "GEDIT_FILE_BROWSER_STORE_COLUMN_FLAGS",
		  "flags" },
		{ GEDIT_FILE_BROWSER_STORE_COLUMN_EMBLEM,
		  "GEDIT_FILE_BROWSER_STORE_COLUMN_EMBLEM",
		  "emblem" },
		{ GEDIT_FILE_BROWSER_STORE_COLUMN_NUM,
		  "GEDIT_FILE_BROWSER_STORE_COLUMN_NUM",
		  "num" },
		{ 0, NULL, NULL }
	};

	gedit_file_browser_store_column_type =
		g_type_module_register_enum (module,
		                               "GeditFileBrowserStoreColumn",
		                               values);

	return gedit_file_browser_store_column_type;
}

GType
gedit_file_browser_store_column_get_type (void)
{
	return gedit_file_browser_store_column_type;
}

static GType gedit_file_browser_store_flag_type = 0;

static GType
register_gedit_file_browser_store_flag (GTypeModule *module)
{
	static const GFlagsValue values[] = {
		{ GEDIT_FILE_BROWSER_STORE_FLAG_IS_DIRECTORY,
		  "GEDIT_FILE_BROWSER_STORE_FLAG_IS_DIRECTORY",
		  "is-directory" },
		{ GEDIT_FILE_BROWSER_STORE_FLAG_IS_HIDDEN,
		  "GEDIT_FILE_BROWSER_STORE_FLAG_IS_HIDDEN",
		  "is-hidden" },
		{ GEDIT_FILE_BROWSER_STORE_FLAG_IS_TEXT,
		  "GEDIT_FILE_BROWSER_STORE_FLAG_IS_TEXT",
		  "is-text" },
		{ GEDIT_FILE_BROWSER_STORE_FLAG_LOADED,
		  "GEDIT_FILE_BROWSER_STORE_FLAG_LOADED",
		  "loaded" },
		{ GEDIT_FILE_BROWSER_STORE_FLAG_IS_FILTERED,
		  "GEDIT_FILE_BROWSER_STORE_FLAG_IS_FILTERED",
		  "is-filtered" },
		{ GEDIT_FILE_BROWSER_STORE_FLAG_IS_DUMMY,
		  "GEDIT_FILE_BROWSER_STORE_FLAG_IS_DUMMY",
		  "is-dummy" },
		{ 0, NULL, NULL }
	};

	gedit_file_browser_store_flag_type =
		g_type_module_register_flags (module,
		                               "GeditFileBrowserStoreFlag",
		                               values);

	return gedit_file_browser_store_flag_type;
}

GType
gedit_file_browser_store_flag_get_type (void)
{
	return gedit_file_browser_store_flag_type;
}

static GType gedit_file_browser_store_result_type = 0;

static GType
register_gedit_file_browser_store_result (GTypeModule *module)
{
	static const GEnumValue values[] = {
		{ GEDIT_FILE_BROWSER_STORE_RESULT_OK,
		  "GEDIT_FILE_BROWSER_STORE_RESULT_OK",
		  "ok" },
		{ GEDIT_FILE_BROWSER_STORE_RESULT_NO_CHANGE,
		  "GEDIT_FILE_BROWSER_STORE_RESULT_NO_CHANGE",
		  "no-change" },
		{ GEDIT_FILE_BROWSER_STORE_RESULT_ERROR,
		  "GEDIT_FILE_BROWSER_STORE_RESULT_ERROR",
		  "error" },
		{ GEDIT_FILE_BROWSER_STORE_RESULT_NO_TRASH,
		  "GEDIT_FILE_BROWSER_STORE_RESULT_NO_TRASH",
		  "no-trash" },
		{ GEDIT_FILE_BROWSER_STORE_RESULT_MOUNTING,
		  "GEDIT_FILE_BROWSER_STORE_RESULT_MOUNTING",
		  "mounting" },
		{ GEDIT_FILE_BROWSER_STORE_RESULT_NUM,
		  "GEDIT_FILE_BROWSER_STORE_RESULT_NUM",
		  "num" },
		{ 0, NULL, NULL }
	};

	gedit_file_browser_store_result_type =
		g_type_module_register_enum (module,
		                               "GeditFileBrowserStoreResult",
		                               values);

	return gedit_file_browser_store_result_type;
}

GType
gedit_file_browser_store_result_get_type (void)
{
	return gedit_file_browser_store_result_type;
}

static GType gedit_file_browser_store_filter_mode_type = 0;

static GType
register_gedit_file_browser_store_filter_mode (GTypeModule *module)
{
	static const GFlagsValue values[] = {
		{ GEDIT_FILE_BROWSER_STORE_FILTER_MODE_NONE,
		  "GEDIT_FILE_BROWSER_STORE_FILTER_MODE_NONE",
		  "none" },
		{ GEDIT_FILE_BROWSER_STORE_FILTER_MODE_HIDE_HIDDEN,
		  "GEDIT_FILE_BROWSER_STORE_FILTER_MODE_HIDE_HIDDEN",
		  "hide-hidden" },
		{ GEDIT_FILE_BROWSER_STORE_FILTER_MODE_HIDE_BINARY,
		  "GEDIT_FILE_BROWSER_STORE_FILTER_MODE_HIDE_BINARY",
		  "hide-binary" },
		{ 0, NULL, NULL }
	};

	gedit_file_browser_store_filter_mode_type =
		g_type_module_register_flags (module,
		                               "GeditFileBrowserStoreFilterMode",
		                               values);

	return gedit_file_browser_store_filter_mode_type;
}

GType
gedit_file_browser_store_filter_mode_get_type (void)
{
	return gedit_file_browser_store_filter_mode_type;
}

/* enumerations from "gedit-file-browser-view.h" */
#include "gedit-file-browser-view.h"

static GType gedit_file_browser_view_click_policy_type = 0;

static GType
register_gedit_file_browser_view_click_policy (GTypeModule *module)
{
	static const GEnumValue values[] = {
		{ GEDIT_FILE_BROWSER_VIEW_CLICK_POLICY_DOUBLE,
		  "GEDIT_FILE_BROWSER_VIEW_CLICK_POLICY_DOUBLE",
		  "double" },
		{ GEDIT_FILE_BROWSER_VIEW_CLICK_POLICY_SINGLE,
		  "GEDIT_FILE_BROWSER_VIEW_CLICK_POLICY_SINGLE",
		  "single" },
		{ 0, NULL, NULL }
	};

	gedit_file_browser_view_click_policy_type =
		g_type_module_register_enum (module,
		                               "GeditFileBrowserViewClickPolicy",
		                               values);

	return gedit_file_browser_view_click_policy_type;
}

GType
gedit_file_browser_view_click_policy_get_type (void)
{
	return gedit_file_browser_view_click_policy_type;
}

/* enumerations from "gedit-file-browser-error.h" */
#include "gedit-file-browser-error.h"

static GType gedit_file_browser_error_type = 0;

static GType
register_gedit_file_browser_error (GTypeModule *module)
{
	static const GEnumValue values[] = {
		{ GEDIT_FILE_BROWSER_ERROR_NONE,
		  "GEDIT_FILE_BROWSER_ERROR_NONE",
		  "none" },
		{ GEDIT_FILE_BROWSER_ERROR_RENAME,
		  "GEDIT_FILE_BROWSER_ERROR_RENAME",
		  "rename" },
		{ GEDIT_FILE_BROWSER_ERROR_DELETE,
		  "GEDIT_FILE_BROWSER_ERROR_DELETE",
		  "delete" },
		{ GEDIT_FILE_BROWSER_ERROR_NEW_FILE,
		  "GEDIT_FILE_BROWSER_ERROR_NEW_FILE",
		  "new-file" },
		{ GEDIT_FILE_BROWSER_ERROR_NEW_DIRECTORY,
		  "GEDIT_FILE_BROWSER_ERROR_NEW_DIRECTORY",
		  "new-directory" },
		{ GEDIT_FILE_BROWSER_ERROR_OPEN_DIRECTORY,
		  "GEDIT_FILE_BROWSER_ERROR_OPEN_DIRECTORY",
		  "open-directory" },
		{ GEDIT_FILE_BROWSER_ERROR_SET_ROOT,
		  "GEDIT_FILE_BROWSER_ERROR_SET_ROOT",
		  "set-root" },
		{ GEDIT_FILE_BROWSER_ERROR_LOAD_DIRECTORY,
		  "GEDIT_FILE_BROWSER_ERROR_LOAD_DIRECTORY",
		  "load-directory" },
		{ GEDIT_FILE_BROWSER_ERROR_NUM,
		  "GEDIT_FILE_BROWSER_ERROR_NUM",
		  "num" },
		{ 0, NULL, NULL }
	};

	gedit_file_browser_error_type =
		g_type_module_register_enum (module,
		                               "GeditFileBrowserError",
		                               values);

	return gedit_file_browser_error_type;
}

GType
gedit_file_browser_error_get_type (void)
{
	return gedit_file_browser_error_type;
}


/* Generated data ends here */


/* Generated data (by glib-mkenums) */

void
gedit_file_browser_enum_and_flag_register_type (GTypeModule * module)
{
	/* Enumerations from "gedit-file-browser-store.h" */
	
	register_gedit_file_browser_store_column (module);

	register_gedit_file_browser_store_flag (module);

	register_gedit_file_browser_store_result (module);

	register_gedit_file_browser_store_filter_mode (module);

	/* Enumerations from "gedit-file-browser-view.h" */
	
	register_gedit_file_browser_view_click_policy (module);

	/* Enumerations from "gedit-file-browser-error.h" */
	
	register_gedit_file_browser_error (module);

}


/* Generated data ends here */


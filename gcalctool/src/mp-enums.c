
/* Generated data (by glib-mkenums) */

#include "mp-serializer.h"
#include "mp-enums.h"

/* enumerations from "./mp-serializer.h" */
GType
math_mp_display_format_get_type (void)
{
    static GType etype = 0;
    if (G_UNLIKELY(etype == 0)) {
        static const GEnumValue values[] = {
            { MP_DISPLAY_FORMAT_AUTOMATIC, "MP_DISPLAY_FORMAT_AUTOMATIC", "automatic" },
            { MP_DISPLAY_FORMAT_FIXED, "MP_DISPLAY_FORMAT_FIXED", "fixed" },
            { MP_DISPLAY_FORMAT_SCIENTIFIC, "MP_DISPLAY_FORMAT_SCIENTIFIC", "scientific" },
            { MP_DISPLAY_FORMAT_ENGINEERING, "MP_DISPLAY_FORMAT_ENGINEERING", "engineering" },
            { 0, NULL, NULL }
        };
        etype = g_enum_register_static (g_intern_static_string ("MpDisplayFormat"), values);
    }
    return etype;
}



/* Generated data ends here */



#ifndef __gdu_marshal_MARSHAL_H__
#define __gdu_marshal_MARSHAL_H__

#include	<glib-object.h>

G_BEGIN_DECLS

/* VOID:BOOLEAN,STRING,UINT,BOOLEAN,DOUBLE (gdu-marshal.list:1) */
extern void gdu_marshal_VOID__BOOLEAN_STRING_UINT_BOOLEAN_DOUBLE (GClosure     *closure,
                                                                  GValue       *return_value,
                                                                  guint         n_param_values,
                                                                  const GValue *param_values,
                                                                  gpointer      invocation_hint,
                                                                  gpointer      marshal_data);

/* VOID:STRING,BOOLEAN,STRING,UINT,BOOLEAN,DOUBLE (gdu-marshal.list:2) */
extern void gdu_marshal_VOID__STRING_BOOLEAN_STRING_UINT_BOOLEAN_DOUBLE (GClosure     *closure,
                                                                         GValue       *return_value,
                                                                         guint         n_param_values,
                                                                         const GValue *param_values,
                                                                         gpointer      invocation_hint,
                                                                         gpointer      marshal_data);

/* STRING:OBJECT (gdu-marshal.list:3) */
extern void gdu_marshal_STRING__OBJECT (GClosure     *closure,
                                        GValue       *return_value,
                                        guint         n_param_values,
                                        const GValue *param_values,
                                        gpointer      invocation_hint,
                                        gpointer      marshal_data);

G_END_DECLS

#endif /* __gdu_marshal_MARSHAL_H__ */


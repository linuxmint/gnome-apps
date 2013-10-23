
/* Generated data (by glib-mkenums) */

#include <gdu-gtk/gdu-gtk.h>

/* enumerations from "gdu-gtk-enums.h" */
GType
gdu_pool_tree_model_column_get_type (void)
{
  static volatile gsize g_define_type_id__volatile = 0;

  if (g_once_init_enter (&g_define_type_id__volatile))
    {
      static const GEnumValue values[] = {
        { GDU_POOL_TREE_MODEL_COLUMN_ICON, "GDU_POOL_TREE_MODEL_COLUMN_ICON", "icon" },
        { GDU_POOL_TREE_MODEL_COLUMN_NAME, "GDU_POOL_TREE_MODEL_COLUMN_NAME", "name" },
        { GDU_POOL_TREE_MODEL_COLUMN_VPD_NAME, "GDU_POOL_TREE_MODEL_COLUMN_VPD_NAME", "vpd-name" },
        { GDU_POOL_TREE_MODEL_COLUMN_DESCRIPTION, "GDU_POOL_TREE_MODEL_COLUMN_DESCRIPTION", "description" },
        { GDU_POOL_TREE_MODEL_COLUMN_PRESENTABLE, "GDU_POOL_TREE_MODEL_COLUMN_PRESENTABLE", "presentable" },
        { GDU_POOL_TREE_MODEL_COLUMN_VISIBLE, "GDU_POOL_TREE_MODEL_COLUMN_VISIBLE", "visible" },
        { GDU_POOL_TREE_MODEL_COLUMN_TOGGLED, "GDU_POOL_TREE_MODEL_COLUMN_TOGGLED", "toggled" },
        { GDU_POOL_TREE_MODEL_COLUMN_CAN_BE_TOGGLED, "GDU_POOL_TREE_MODEL_COLUMN_CAN_BE_TOGGLED", "can-be-toggled" },
        { 0, NULL, NULL }
      };
      GType g_define_type_id =
        g_enum_register_static (g_intern_static_string ("GduPoolTreeModelColumn"), values);
      g_once_init_leave (&g_define_type_id__volatile, g_define_type_id);
    }

  return g_define_type_id__volatile;
}

GType
gdu_pool_tree_view_flags_get_type (void)
{
  static volatile gsize g_define_type_id__volatile = 0;

  if (g_once_init_enter (&g_define_type_id__volatile))
    {
      static const GFlagsValue values[] = {
        { GDU_POOL_TREE_VIEW_FLAGS_NONE, "GDU_POOL_TREE_VIEW_FLAGS_NONE", "none" },
        { GDU_POOL_TREE_VIEW_FLAGS_SHOW_TOGGLE, "GDU_POOL_TREE_VIEW_FLAGS_SHOW_TOGGLE", "show-toggle" },
        { 0, NULL, NULL }
      };
      GType g_define_type_id =
        g_flags_register_static (g_intern_static_string ("GduPoolTreeViewFlags"), values);
      g_once_init_leave (&g_define_type_id__volatile, g_define_type_id);
    }

  return g_define_type_id__volatile;
}

GType
gdu_pool_tree_model_flags_get_type (void)
{
  static volatile gsize g_define_type_id__volatile = 0;

  if (g_once_init_enter (&g_define_type_id__volatile))
    {
      static const GFlagsValue values[] = {
        { GDU_POOL_TREE_MODEL_FLAGS_NONE, "GDU_POOL_TREE_MODEL_FLAGS_NONE", "none" },
        { GDU_POOL_TREE_MODEL_FLAGS_NO_VOLUMES, "GDU_POOL_TREE_MODEL_FLAGS_NO_VOLUMES", "no-volumes" },
        { GDU_POOL_TREE_MODEL_FLAGS_NO_UNALLOCATABLE_DRIVES, "GDU_POOL_TREE_MODEL_FLAGS_NO_UNALLOCATABLE_DRIVES", "no-unallocatable-drives" },
        { 0, NULL, NULL }
      };
      GType g_define_type_id =
        g_flags_register_static (g_intern_static_string ("GduPoolTreeModelFlags"), values);
      g_once_init_leave (&g_define_type_id__volatile, g_define_type_id);
    }

  return g_define_type_id__volatile;
}

GType
gdu_format_dialog_flags_get_type (void)
{
  static volatile gsize g_define_type_id__volatile = 0;

  if (g_once_init_enter (&g_define_type_id__volatile))
    {
      static const GFlagsValue values[] = {
        { GDU_FORMAT_DIALOG_FLAGS_NONE, "GDU_FORMAT_DIALOG_FLAGS_NONE", "none" },
        { GDU_FORMAT_DIALOG_FLAGS_SIMPLE, "GDU_FORMAT_DIALOG_FLAGS_SIMPLE", "simple" },
        { GDU_FORMAT_DIALOG_FLAGS_DISK_UTILITY_BUTTON, "GDU_FORMAT_DIALOG_FLAGS_DISK_UTILITY_BUTTON", "disk-utility-button" },
        { GDU_FORMAT_DIALOG_FLAGS_ALLOW_MSDOS_EXTENDED, "GDU_FORMAT_DIALOG_FLAGS_ALLOW_MSDOS_EXTENDED", "allow-msdos-extended" },
        { 0, NULL, NULL }
      };
      GType g_define_type_id =
        g_flags_register_static (g_intern_static_string ("GduFormatDialogFlags"), values);
      g_once_init_leave (&g_define_type_id__volatile, g_define_type_id);
    }

  return g_define_type_id__volatile;
}

GType
gdu_disk_selection_widget_flags_get_type (void)
{
  static volatile gsize g_define_type_id__volatile = 0;

  if (g_once_init_enter (&g_define_type_id__volatile))
    {
      static const GFlagsValue values[] = {
        { GDU_DISK_SELECTION_WIDGET_FLAGS_NONE, "GDU_DISK_SELECTION_WIDGET_FLAGS_NONE", "none" },
        { GDU_DISK_SELECTION_WIDGET_FLAGS_ALLOW_MULTIPLE, "GDU_DISK_SELECTION_WIDGET_FLAGS_ALLOW_MULTIPLE", "allow-multiple" },
        { GDU_DISK_SELECTION_WIDGET_FLAGS_ALLOW_DISKS_WITH_INSUFFICIENT_SPACE, "GDU_DISK_SELECTION_WIDGET_FLAGS_ALLOW_DISKS_WITH_INSUFFICIENT_SPACE", "allow-disks-with-insufficient-space" },
        { 0, NULL, NULL }
      };
      GType g_define_type_id =
        g_flags_register_static (g_intern_static_string ("GduDiskSelectionWidgetFlags"), values);
      g_once_init_leave (&g_define_type_id__volatile, g_define_type_id);
    }

  return g_define_type_id__volatile;
}

GType
gdu_add_component_linux_md_flags_get_type (void)
{
  static volatile gsize g_define_type_id__volatile = 0;

  if (g_once_init_enter (&g_define_type_id__volatile))
    {
      static const GFlagsValue values[] = {
        { GDU_ADD_COMPONENT_LINUX_MD_FLAGS_NONE, "GDU_ADD_COMPONENT_LINUX_MD_FLAGS_NONE", "none" },
        { GDU_ADD_COMPONENT_LINUX_MD_FLAGS_SPARE, "GDU_ADD_COMPONENT_LINUX_MD_FLAGS_SPARE", "spare" },
        { GDU_ADD_COMPONENT_LINUX_MD_FLAGS_EXPANSION, "GDU_ADD_COMPONENT_LINUX_MD_FLAGS_EXPANSION", "expansion" },
        { 0, NULL, NULL }
      };
      GType g_define_type_id =
        g_flags_register_static (g_intern_static_string ("GduAddComponentLinuxMdFlags"), values);
      g_once_init_leave (&g_define_type_id__volatile, g_define_type_id);
    }

  return g_define_type_id__volatile;
}


/* Generated data ends here */


/* A Bison parser, made by GNU Bison 2.5.  */

/* Bison implementation for Yacc-like parsers in C
   
      Copyright (C) 1984, 1989-1990, 2000-2011 Free Software Foundation, Inc.
   
   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.
   
   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
   
   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.  */

/* As a special exception, you may create a larger work that contains
   part or all of the Bison parser skeleton and distribute that work
   under terms of your choice, so long as that work isn't itself a
   parser generator using the skeleton or a modified version thereof
   as a parser skeleton.  Alternatively, if you modify or redistribute
   the parser skeleton itself, you may (at your option) remove this
   special exception, which will cause the skeleton and the resulting
   Bison output files to be licensed under the GNU General Public
   License without this special exception.
   
   This special exception was added by the Free Software Foundation in
   version 2.2 of Bison.  */

/* C LALR(1) parser skeleton written by Richard Stallman, by
   simplifying the original so-called "semantic" parser.  */

/* All symbols defined below should begin with yy or YY, to avoid
   infringing on user name space.  This should be done even for local
   variables, as they might otherwise be expanded by user macros.
   There are some unavoidable exceptions within include files to
   define necessary library symbols; they are noted "INFRINGES ON
   USER NAME SPACE" below.  */

/* Identify Bison output.  */
#define YYBISON 1

/* Bison version.  */
#define YYBISON_VERSION "2.5"

/* Skeleton name.  */
#define YYSKELETON_NAME "yacc.c"

/* Pure parsers.  */
#define YYPURE 1

/* Push parsers.  */
#define YYPUSH 0

/* Pull parsers.  */
#define YYPULL 1

/* Using locations.  */
#define YYLSP_NEEDED 1

/* Substitute the variable and function names.  */
#define yyparse         _mp_equation_parse
#define yylex           _mp_equation_lex
#define yyerror         _mp_equation_error
#define yylval          _mp_equation_lval
#define yychar          _mp_equation_char
#define yydebug         _mp_equation_debug
#define yynerrs         _mp_equation_nerrs
#define yylloc          _mp_equation_lloc

/* Copy the first part of user declarations.  */

/* Line 268 of yacc.c  */
#line 1 "./mp-equation-parser.y"

/*
 * Copyright (C) 2004-2008 Sami Pietila
 * Copyright (C) 2008-2011 Robert Ancell
 *
 * This program is free software: you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 2 of the License, or (at your option) any later
 * version. See http://www.gnu.org/copyleft/gpl.html the full text of the
 * license.
 */

#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <errno.h>
#include <assert.h>

#include "mp-equation-private.h"
#include "mp-equation-parser.h"
#include "mp-equation-lexer.h"

// fixme support x log x
// treat exp NAME exp as a function always and pass both arguments, i.e.
// can do mod using both and all others use $1 * NAME($3)

static void set_error(yyscan_t yyscanner, int error, const char *token)
{
    _mp_equation_get_extra(yyscanner)->error = error;
    if (token)
        _mp_equation_get_extra(yyscanner)->error_token = strdup(token);
}

static void set_result(yyscan_t yyscanner, const MPNumber *x)
{
    mp_set_from_mp(x, &(_mp_equation_get_extra(yyscanner))->ret);
}

static char *
utf8_next_char (const char *c)
{
    c++;
    while ((*c & 0xC0) == 0x80)
        c++;
    return (char *)c;
}

static int get_variable(yyscan_t yyscanner, const char *name, int power, MPNumber *z)
{
    int result = 0;

    /* If defined, then get the variable */
    if (_mp_equation_get_extra(yyscanner)->get_variable(_mp_equation_get_extra(yyscanner), name, z)) {
        mp_xpowy_integer(z, power, z);
        return 1;
    }
    
    /* If has more than one character then assume a multiplication of variables */
    if (utf8_next_char(name)[0] != '\0') {
        const char *c, *next;
        char *buffer = malloc(sizeof(char) * strlen(name));
        MPNumber value;

        result = 1;
        mp_set_from_integer(1, &value);
        for (c = name; *c != '\0'; c = next) {
            MPNumber t;

            next = utf8_next_char(c);
            snprintf(buffer, next - c + 1, "%s", c);

            if (!_mp_equation_get_extra(yyscanner)->get_variable(_mp_equation_get_extra(yyscanner), buffer, &t)) {
                result = 0;
                break;
            }

            /* If last term do power */
            if (*next == '\0')
                mp_xpowy_integer(&t, power, &t);

            mp_multiply(&value, &t, &value);
        }

        free(buffer);
        if (result)
            mp_set_from_mp(&value, z);
    }

    if (!result)
        set_error(yyscanner, PARSER_ERR_UNKNOWN_VARIABLE, name);

    return result;
}

static void set_variable(yyscan_t yyscanner, const char *name, MPNumber *x)
{
    _mp_equation_get_extra(yyscanner)->set_variable(_mp_equation_get_extra(yyscanner), name, x);
}

static int get_function(yyscan_t yyscanner, const char *name, const MPNumber *x, MPNumber *z)
{
    if (!_mp_equation_get_extra(yyscanner)->get_function(_mp_equation_get_extra(yyscanner), name, x, z)) {
        set_error(yyscanner, PARSER_ERR_UNKNOWN_FUNCTION, name);
        return 0;
    }
    return 1;
}

static int get_inverse_function(yyscan_t yyscanner, const char *name, const MPNumber *x, MPNumber *z)
{
    char *inv_name;
    int result;
    
    inv_name = malloc(sizeof(char) * (strlen(name) + strlen("⁻¹") + 1));
    strcpy(inv_name, name);
    strcat(inv_name, "⁻¹");
    result = get_function(yyscanner, inv_name, x, z);
    free(inv_name);

    return result;
}

static void do_not(yyscan_t yyscanner, const MPNumber *x, MPNumber *z)
{
    if (!mp_is_overflow(x, _mp_equation_get_extra(yyscanner)->options->wordlen)) {
        set_error(yyscanner, PARSER_ERR_OVERFLOW, NULL);
    }
    mp_not(x, _mp_equation_get_extra(yyscanner)->options->wordlen, z);
}

static char *make_unit(const char *name, int power)
{
    char *name2;

    // FIXME: Hacky
    if (power == 2) {
        name2 = malloc(sizeof(char) * (strlen(name) + strlen("²") + 1));
        sprintf(name2, "%s²", name);
    }
    else if (power == 3) {
        name2 = malloc(sizeof(char) * (strlen(name) + strlen("³") + 1));
        sprintf(name2, "%s³", name);
    }
    else {
        name2 = malloc(sizeof(char) * (strlen(name) + strlen("?") + 1));
        sprintf(name2, "%s?", name);
    }
    
    return name2;
}

static void do_conversion(yyscan_t yyscanner, const MPNumber *x, const char *x_units, const char *z_units, MPNumber *z)
{
    if (!_mp_equation_get_extra(yyscanner)->convert(_mp_equation_get_extra(yyscanner), x, x_units, z_units, z))
        set_error(yyscanner, PARSER_ERR_UNKNOWN_CONVERSION, NULL);
}



/* Line 268 of yacc.c  */
#line 239 "mp-equation-parser.c"

/* Enabling traces.  */
#ifndef YYDEBUG
# define YYDEBUG 0
#endif

/* Enabling verbose error messages.  */
#ifdef YYERROR_VERBOSE
# undef YYERROR_VERBOSE
# define YYERROR_VERBOSE 1
#else
# define YYERROR_VERBOSE 0
#endif

/* Enabling the token table.  */
#ifndef YYTOKEN_TABLE
# define YYTOKEN_TABLE 0
#endif


/* Tokens.  */
#ifndef YYTOKENTYPE
# define YYTOKENTYPE
   /* Put the tokens into the symbol table, so that GDB and other debuggers
      know about them.  */
   enum yytokentype {
     tNUMBER = 258,
     tRCEILING = 259,
     tLCEILING = 260,
     tRFLOOR = 261,
     tLFLOOR = 262,
     UNARY_PLUS = 263,
     tSUBTRACT = 264,
     tADD = 265,
     tXNOR = 266,
     tXOR = 267,
     tOR = 268,
     tAND = 269,
     MULTIPLICATION = 270,
     tMULTIPLY = 271,
     tMOD = 272,
     tDIVIDE = 273,
     tNOT = 274,
     tROOT4 = 275,
     tROOT3 = 276,
     tROOT = 277,
     tFUNCTION = 278,
     tVARIABLE = 279,
     tNSUPNUM = 280,
     tSUPNUM = 281,
     tSUBNUM = 282,
     BOOLEAN_OPERATOR = 283,
     PERCENTAGE = 284,
     UNARY_MINUS = 285,
     tIN = 286
   };
#endif
/* Tokens.  */
#define tNUMBER 258
#define tRCEILING 259
#define tLCEILING 260
#define tRFLOOR 261
#define tLFLOOR 262
#define UNARY_PLUS 263
#define tSUBTRACT 264
#define tADD 265
#define tXNOR 266
#define tXOR 267
#define tOR 268
#define tAND 269
#define MULTIPLICATION 270
#define tMULTIPLY 271
#define tMOD 272
#define tDIVIDE 273
#define tNOT 274
#define tROOT4 275
#define tROOT3 276
#define tROOT 277
#define tFUNCTION 278
#define tVARIABLE 279
#define tNSUPNUM 280
#define tSUPNUM 281
#define tSUBNUM 282
#define BOOLEAN_OPERATOR 283
#define PERCENTAGE 284
#define UNARY_MINUS 285
#define tIN 286




#if ! defined YYSTYPE && ! defined YYSTYPE_IS_DECLARED
typedef union YYSTYPE
{

/* Line 293 of yacc.c  */
#line 166 "./mp-equation-parser.y"

  MPNumber int_t;
  int integer;
  char *name;



/* Line 293 of yacc.c  */
#line 345 "mp-equation-parser.c"
} YYSTYPE;
# define YYSTYPE_IS_TRIVIAL 1
# define yystype YYSTYPE /* obsolescent; will be withdrawn */
# define YYSTYPE_IS_DECLARED 1
#endif

#if ! defined YYLTYPE && ! defined YYLTYPE_IS_DECLARED
typedef struct YYLTYPE
{
  int first_line;
  int first_column;
  int last_line;
  int last_column;
} YYLTYPE;
# define yyltype YYLTYPE /* obsolescent; will be withdrawn */
# define YYLTYPE_IS_DECLARED 1
# define YYLTYPE_IS_TRIVIAL 1
#endif


/* Copy the second part of user declarations.  */


/* Line 343 of yacc.c  */
#line 370 "mp-equation-parser.c"

#ifdef short
# undef short
#endif

#ifdef YYTYPE_UINT8
typedef YYTYPE_UINT8 yytype_uint8;
#else
typedef unsigned char yytype_uint8;
#endif

#ifdef YYTYPE_INT8
typedef YYTYPE_INT8 yytype_int8;
#elif (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
typedef signed char yytype_int8;
#else
typedef short int yytype_int8;
#endif

#ifdef YYTYPE_UINT16
typedef YYTYPE_UINT16 yytype_uint16;
#else
typedef unsigned short int yytype_uint16;
#endif

#ifdef YYTYPE_INT16
typedef YYTYPE_INT16 yytype_int16;
#else
typedef short int yytype_int16;
#endif

#ifndef YYSIZE_T
# ifdef __SIZE_TYPE__
#  define YYSIZE_T __SIZE_TYPE__
# elif defined size_t
#  define YYSIZE_T size_t
# elif ! defined YYSIZE_T && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
#  include <stddef.h> /* INFRINGES ON USER NAME SPACE */
#  define YYSIZE_T size_t
# else
#  define YYSIZE_T unsigned int
# endif
#endif

#define YYSIZE_MAXIMUM ((YYSIZE_T) -1)

#ifndef YY_
# if defined YYENABLE_NLS && YYENABLE_NLS
#  if ENABLE_NLS
#   include <libintl.h> /* INFRINGES ON USER NAME SPACE */
#   define YY_(msgid) dgettext ("bison-runtime", msgid)
#  endif
# endif
# ifndef YY_
#  define YY_(msgid) msgid
# endif
#endif

/* Suppress unused-variable warnings by "using" E.  */
#if ! defined lint || defined __GNUC__
# define YYUSE(e) ((void) (e))
#else
# define YYUSE(e) /* empty */
#endif

/* Identity function, used to suppress warnings about constant conditions.  */
#ifndef lint
# define YYID(n) (n)
#else
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static int
YYID (int yyi)
#else
static int
YYID (yyi)
    int yyi;
#endif
{
  return yyi;
}
#endif

#if ! defined yyoverflow || YYERROR_VERBOSE

/* The parser invokes alloca or malloc; define the necessary symbols.  */

# ifdef YYSTACK_USE_ALLOCA
#  if YYSTACK_USE_ALLOCA
#   ifdef __GNUC__
#    define YYSTACK_ALLOC __builtin_alloca
#   elif defined __BUILTIN_VA_ARG_INCR
#    include <alloca.h> /* INFRINGES ON USER NAME SPACE */
#   elif defined _AIX
#    define YYSTACK_ALLOC __alloca
#   elif defined _MSC_VER
#    include <malloc.h> /* INFRINGES ON USER NAME SPACE */
#    define alloca _alloca
#   else
#    define YYSTACK_ALLOC alloca
#    if ! defined _ALLOCA_H && ! defined EXIT_SUCCESS && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
#     include <stdlib.h> /* INFRINGES ON USER NAME SPACE */
#     ifndef EXIT_SUCCESS
#      define EXIT_SUCCESS 0
#     endif
#    endif
#   endif
#  endif
# endif

# ifdef YYSTACK_ALLOC
   /* Pacify GCC's `empty if-body' warning.  */
#  define YYSTACK_FREE(Ptr) do { /* empty */; } while (YYID (0))
#  ifndef YYSTACK_ALLOC_MAXIMUM
    /* The OS might guarantee only one guard page at the bottom of the stack,
       and a page size can be as small as 4096 bytes.  So we cannot safely
       invoke alloca (N) if N exceeds 4096.  Use a slightly smaller number
       to allow for a few compiler-allocated temporary stack slots.  */
#   define YYSTACK_ALLOC_MAXIMUM 4032 /* reasonable circa 2006 */
#  endif
# else
#  define YYSTACK_ALLOC YYMALLOC
#  define YYSTACK_FREE YYFREE
#  ifndef YYSTACK_ALLOC_MAXIMUM
#   define YYSTACK_ALLOC_MAXIMUM YYSIZE_MAXIMUM
#  endif
#  if (defined __cplusplus && ! defined EXIT_SUCCESS \
       && ! ((defined YYMALLOC || defined malloc) \
	     && (defined YYFREE || defined free)))
#   include <stdlib.h> /* INFRINGES ON USER NAME SPACE */
#   ifndef EXIT_SUCCESS
#    define EXIT_SUCCESS 0
#   endif
#  endif
#  ifndef YYMALLOC
#   define YYMALLOC malloc
#   if ! defined malloc && ! defined EXIT_SUCCESS && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
void *malloc (YYSIZE_T); /* INFRINGES ON USER NAME SPACE */
#   endif
#  endif
#  ifndef YYFREE
#   define YYFREE free
#   if ! defined free && ! defined EXIT_SUCCESS && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
void free (void *); /* INFRINGES ON USER NAME SPACE */
#   endif
#  endif
# endif
#endif /* ! defined yyoverflow || YYERROR_VERBOSE */


#if (! defined yyoverflow \
     && (! defined __cplusplus \
	 || (defined YYLTYPE_IS_TRIVIAL && YYLTYPE_IS_TRIVIAL \
	     && defined YYSTYPE_IS_TRIVIAL && YYSTYPE_IS_TRIVIAL)))

/* A type that is properly aligned for any stack member.  */
union yyalloc
{
  yytype_int16 yyss_alloc;
  YYSTYPE yyvs_alloc;
  YYLTYPE yyls_alloc;
};

/* The size of the maximum gap between one aligned stack and the next.  */
# define YYSTACK_GAP_MAXIMUM (sizeof (union yyalloc) - 1)

/* The size of an array large to enough to hold all stacks, each with
   N elements.  */
# define YYSTACK_BYTES(N) \
     ((N) * (sizeof (yytype_int16) + sizeof (YYSTYPE) + sizeof (YYLTYPE)) \
      + 2 * YYSTACK_GAP_MAXIMUM)

# define YYCOPY_NEEDED 1

/* Relocate STACK from its old location to the new one.  The
   local variables YYSIZE and YYSTACKSIZE give the old and new number of
   elements in the stack, and YYPTR gives the new location of the
   stack.  Advance YYPTR to a properly aligned location for the next
   stack.  */
# define YYSTACK_RELOCATE(Stack_alloc, Stack)				\
    do									\
      {									\
	YYSIZE_T yynewbytes;						\
	YYCOPY (&yyptr->Stack_alloc, Stack, yysize);			\
	Stack = &yyptr->Stack_alloc;					\
	yynewbytes = yystacksize * sizeof (*Stack) + YYSTACK_GAP_MAXIMUM; \
	yyptr += yynewbytes / sizeof (*yyptr);				\
      }									\
    while (YYID (0))

#endif

#if defined YYCOPY_NEEDED && YYCOPY_NEEDED
/* Copy COUNT objects from FROM to TO.  The source and destination do
   not overlap.  */
# ifndef YYCOPY
#  if defined __GNUC__ && 1 < __GNUC__
#   define YYCOPY(To, From, Count) \
      __builtin_memcpy (To, From, (Count) * sizeof (*(From)))
#  else
#   define YYCOPY(To, From, Count)		\
      do					\
	{					\
	  YYSIZE_T yyi;				\
	  for (yyi = 0; yyi < (Count); yyi++)	\
	    (To)[yyi] = (From)[yyi];		\
	}					\
      while (YYID (0))
#  endif
# endif
#endif /* !YYCOPY_NEEDED */

/* YYFINAL -- State number of the termination state.  */
#define YYFINAL  45
/* YYLAST -- Last index in YYTABLE.  */
#define YYLAST   591

/* YYNTOKENS -- Number of terminals.  */
#define YYNTOKENS  43
/* YYNNTS -- Number of nonterminals.  */
#define YYNNTS  6
/* YYNRULES -- Number of rules.  */
#define YYNRULES  50
/* YYNRULES -- Number of states.  */
#define YYNSTATES  101

/* YYTRANSLATE(YYLEX) -- Bison symbol number corresponding to YYLEX.  */
#define YYUNDEFTOK  2
#define YYMAXUTOK   286

#define YYTRANSLATE(YYX)						\
  ((unsigned int) (YYX) <= YYMAXUTOK ? yytranslate[YYX] : YYUNDEFTOK)

/* YYTRANSLATE[YYLEX] -- Bison symbol number corresponding to YYLEX.  */
static const yytype_uint8 yytranslate[] =
{
       0,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,    32,     2,     2,     2,    42,     2,     2,
      36,    37,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,    35,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,    38,     2,    39,    31,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,    40,    33,    41,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     1,     2,     3,     4,
       5,     6,     7,     8,     9,    10,    11,    12,    13,    14,
      15,    16,    17,    18,    19,    20,    21,    22,    23,    24,
      25,    26,    27,    28,    29,    30,    34
};

#if YYDEBUG
/* YYPRHS[YYN] -- Index of the first RHS symbol of rule number YYN in
   YYRHS.  */
static const yytype_uint8 yyprhs[] =
{
       0,     0,     3,     5,     8,    12,    17,    21,    23,    26,
      30,    37,    44,    49,    53,    57,    61,    65,    69,    73,
      76,    79,    82,    84,    87,    90,    93,    97,   101,   105,
     110,   115,   119,   123,   126,   129,   133,   137,   141,   143,
     145,   148,   152,   156,   160,   164,   167,   170,   173,   175,
     178
};

/* YYRHS -- A `-1'-separated list of the rules' RHS.  */
static const yytype_int8 yyrhs[] =
{
      44,     0,    -1,    46,    -1,    46,    35,    -1,    24,    35,
      46,    -1,     3,    45,    34,    45,    -1,    45,    34,    45,
      -1,    24,    -1,    24,    26,    -1,    36,    46,    37,    -1,
      46,    18,    46,    36,    46,    37,    -1,    46,    17,    46,
      36,    46,    37,    -1,    46,    36,    46,    37,    -1,     7,
      46,     6,    -1,     5,    46,     4,    -1,    38,    46,    39,
      -1,    40,    46,    41,    -1,    33,    46,    33,    -1,    46,
      31,    46,    -1,    46,    26,    -1,    46,    25,    -1,    46,
      32,    -1,    47,    -1,     3,    47,    -1,     9,    46,    -1,
      10,     3,    -1,    46,    18,    46,    -1,    46,    17,    46,
      -1,    46,    16,    46,    -1,    46,    10,    46,    42,    -1,
      46,     9,    46,    42,    -1,    46,    10,    46,    -1,    46,
       9,    46,    -1,    46,    42,    -1,    19,    46,    -1,    46,
      14,    46,    -1,    46,    13,    46,    -1,    46,    12,    46,
      -1,     3,    -1,    48,    -1,    23,    46,    -1,    23,    26,
      46,    -1,    23,    25,    46,    -1,    24,    26,    46,    -1,
      27,    22,    46,    -1,    22,    46,    -1,    21,    46,    -1,
      20,    46,    -1,    24,    -1,    24,    26,    -1,    48,    48,
      -1
};

/* YYRLINE[YYN] -- source line where rule number YYN was defined.  */
static const yytype_uint16 yyrline[] =
{
       0,   196,   196,   197,   198,   199,   200,   204,   205,   210,
     211,   212,   213,   214,   215,   216,   217,   218,   219,   220,
     221,   222,   223,   224,   225,   226,   227,   228,   229,   230,
     231,   232,   233,   234,   235,   236,   237,   238,   239,   244,
     245,   246,   247,   248,   249,   250,   251,   252,   256,   257,
     258
};
#endif

#if YYDEBUG || YYERROR_VERBOSE || YYTOKEN_TABLE
/* YYTNAME[SYMBOL-NUM] -- String name of the symbol SYMBOL-NUM.
   First, the terminals, then, starting at YYNTOKENS, nonterminals.  */
static const char *const yytname[] =
{
  "$end", "error", "$undefined", "tNUMBER", "tRCEILING", "tLCEILING",
  "tRFLOOR", "tLFLOOR", "UNARY_PLUS", "tSUBTRACT", "tADD", "tXNOR", "tXOR",
  "tOR", "tAND", "MULTIPLICATION", "tMULTIPLY", "tMOD", "tDIVIDE", "tNOT",
  "tROOT4", "tROOT3", "tROOT", "tFUNCTION", "tVARIABLE", "tNSUPNUM",
  "tSUPNUM", "tSUBNUM", "BOOLEAN_OPERATOR", "PERCENTAGE", "UNARY_MINUS",
  "'^'", "'!'", "'|'", "tIN", "'='", "'('", "')'", "'['", "']'", "'{'",
  "'}'", "'%'", "$accept", "statement", "unit", "exp", "variable", "term", 0
};
#endif

# ifdef YYPRINT
/* YYTOKNUM[YYLEX-NUM] -- Internal token number corresponding to
   token YYLEX-NUM.  */
static const yytype_uint16 yytoknum[] =
{
       0,   256,   257,   258,   259,   260,   261,   262,   263,   264,
     265,   266,   267,   268,   269,   270,   271,   272,   273,   274,
     275,   276,   277,   278,   279,   280,   281,   282,   283,   284,
     285,    94,    33,   124,   286,    61,    40,    41,    91,    93,
     123,   125,    37
};
# endif

/* YYR1[YYN] -- Symbol number of symbol that rule YYN derives.  */
static const yytype_uint8 yyr1[] =
{
       0,    43,    44,    44,    44,    44,    44,    45,    45,    46,
      46,    46,    46,    46,    46,    46,    46,    46,    46,    46,
      46,    46,    46,    46,    46,    46,    46,    46,    46,    46,
      46,    46,    46,    46,    46,    46,    46,    46,    46,    47,
      47,    47,    47,    47,    47,    47,    47,    47,    48,    48,
      48
};

/* YYR2[YYN] -- Number of symbols composing right hand side of rule YYN.  */
static const yytype_uint8 yyr2[] =
{
       0,     2,     1,     2,     3,     4,     3,     1,     2,     3,
       6,     6,     4,     3,     3,     3,     3,     3,     3,     2,
       2,     2,     1,     2,     2,     2,     3,     3,     3,     4,
       4,     3,     3,     2,     2,     3,     3,     3,     1,     1,
       2,     3,     3,     3,     3,     2,     2,     2,     1,     2,
       2
};

/* YYDEFACT[STATE-NAME] -- Default reduction number in state STATE-NUM.
   Performed when YYTABLE doesn't specify something else to do.  Zero
   means the default is an error.  */
static const yytype_uint8 yydefact[] =
{
       0,    38,     0,     0,     0,     0,     0,     0,     0,     0,
       0,    48,     0,     0,     0,     0,     0,     0,     0,     2,
      22,    39,    48,     0,    23,    38,    48,     0,     0,    24,
      25,    34,    47,    46,    45,     0,     0,    40,    49,     0,
       0,     0,     0,     0,     0,     1,     0,     0,     0,     0,
       0,     0,     0,     0,     0,    20,    19,     0,    21,     3,
       0,    33,    48,    50,     0,    49,    14,    13,    42,    41,
      43,     4,    44,    17,     9,    15,    16,     7,     6,    32,
      31,    37,    36,    35,    28,    27,    26,    18,     0,    49,
       5,     8,    30,    29,     0,     0,    12,     0,     0,    11,
      10
};

/* YYDEFGOTO[NTERM-NUM].  */
static const yytype_int8 yydefgoto[] =
{
      -1,    17,    18,    70,    20,    21
};

/* YYPACT[STATE-NUM] -- Index in YYTABLE of the portion describing
   STATE-NUM.  */
#define YYPACT_NINF -21
static const yytype_int16 yypact[] =
{
     119,    85,   144,   144,   144,     9,   144,   144,   144,   144,
      94,    11,    -4,   144,   144,   144,   144,    23,    -3,   254,
     -21,     6,    -9,     7,   -21,   564,    -2,    54,   223,   -10,
     -21,    51,    51,    51,    51,   144,   144,    51,   167,   144,
     144,   285,   316,   347,   378,   -21,    18,   144,   144,   144,
     144,   144,   144,   144,   144,   -21,   -21,   144,   -21,   -21,
     144,   -21,    29,     6,    18,   190,   -21,   -21,    51,    51,
      51,   505,    51,   -21,   -21,   -21,   -21,    33,   -21,   526,
     547,   -10,   -10,   -10,     2,   189,   220,   -10,   412,   -21,
     -21,   -21,   -21,   -21,   144,   144,   -21,   443,   474,   -21,
     -21
};

/* YYPGOTO[NTERM-NUM].  */
static const yytype_int8 yypgoto[] =
{
     -21,   -21,    10,     0,     4,   -20
};

/* YYTABLE[YYPACT[STATE-NUM]].  What to do in state STATE-NUM.  If
   positive, shift that token.  If negative, reduce the rule which
   number is the opposite.  If YYTABLE_NINF, syntax error.  */
#define YYTABLE_NINF -9
static const yytype_int8 yytable[] =
{
      19,    63,    27,    28,    29,    24,    31,    32,    33,    34,
      37,    23,    30,    41,    42,    43,    44,    38,    40,    53,
      54,    57,    58,    45,    65,    -7,    60,    55,    56,    24,
      62,    46,    61,    57,    58,    68,    69,    38,    60,    71,
      72,    64,    77,    63,    61,    -7,    39,    79,    80,    81,
      82,    83,    84,    85,    86,    89,    78,    87,    66,    91,
      88,     0,     0,    47,    48,     0,    49,    50,    51,     0,
      52,    53,    54,     0,    90,     0,    55,    56,     0,    55,
      56,     0,    57,    58,     0,    57,    58,    60,     0,     0,
      60,     0,     0,    61,    97,    98,    61,    25,     0,     2,
       0,     3,     0,     4,     5,     7,     8,     9,    10,    22,
       0,     0,    12,     6,     7,     8,     9,    10,    26,    35,
      36,    12,     1,     0,     2,     0,     3,    13,     4,     5,
      14,     0,    15,     0,    16,     0,     0,     0,     6,     7,
       8,     9,    10,    11,     0,     0,    12,    25,     0,     2,
       0,     3,    13,     4,     5,    14,     0,    15,     0,    16,
       0,     0,     0,     6,     7,     8,     9,    10,    26,     0,
      25,    12,     2,     0,     3,     0,     0,    13,     0,     0,
      14,     0,    15,     0,    16,     0,     6,     7,     8,     9,
      10,     0,     0,    25,    12,     2,     0,     3,     0,     0,
      13,    -8,     0,    14,     0,    15,     0,    16,     0,     6,
       7,     8,     9,    10,    55,    56,     0,    12,     0,     0,
      57,    58,     0,    13,     0,    94,    14,     0,    15,    67,
      16,    61,    47,    48,     0,    49,    50,    51,     0,    52,
      53,    54,     0,     0,     0,    55,    56,     0,    55,    56,
       0,    57,    58,     0,    57,    58,    95,     0,     0,    60,
       0,     0,    61,    47,    48,    61,    49,    50,    51,     0,
      52,    53,    54,     0,     0,     0,     0,     0,     0,    55,
      56,     0,     0,     0,     0,    57,    58,     0,     0,    59,
      60,     0,     0,     0,    47,    48,    61,    49,    50,    51,
       0,    52,    53,    54,     0,     0,     0,     0,     0,     0,
      55,    56,     0,     0,     0,     0,    57,    58,    73,     0,
       0,    60,     0,     0,     0,    47,    48,    61,    49,    50,
      51,     0,    52,    53,    54,     0,     0,     0,     0,     0,
       0,    55,    56,     0,     0,     0,     0,    57,    58,     0,
       0,     0,    60,    74,     0,     0,    47,    48,    61,    49,
      50,    51,     0,    52,    53,    54,     0,     0,     0,     0,
       0,     0,    55,    56,     0,     0,     0,     0,    57,    58,
       0,     0,     0,    60,     0,     0,    75,    47,    48,    61,
      49,    50,    51,     0,    52,    53,    54,     0,     0,     0,
       0,     0,     0,    55,    56,     0,     0,     0,     0,    57,
      58,     0,     0,     0,    60,     0,     0,     0,     0,    76,
      61,    47,    48,     0,    49,    50,    51,     0,    52,    53,
      54,     0,     0,     0,     0,     0,     0,    55,    56,     0,
       0,     0,     0,    57,    58,     0,     0,     0,    60,    96,
       0,     0,    47,    48,    61,    49,    50,    51,     0,    52,
      53,    54,     0,     0,     0,     0,     0,     0,    55,    56,
       0,     0,     0,     0,    57,    58,     0,     0,     0,    60,
      99,     0,     0,    47,    48,    61,    49,    50,    51,     0,
      52,    53,    54,     0,     0,     0,     0,     0,     0,    55,
      56,     0,     0,     0,     0,    57,    58,     0,     0,     0,
      60,   100,     0,     0,    47,    48,    61,    49,    50,    51,
       0,    52,    53,    54,     0,     0,     0,     0,     0,     0,
      55,    56,     0,     0,     0,     0,    57,    58,    49,    50,
      51,    60,    52,    53,    54,     0,     0,    61,     0,     0,
       0,    55,    56,     0,     0,     0,     0,    57,    58,    49,
      50,    51,    60,    52,    53,    54,     0,     0,    92,     0,
       0,     0,    55,    56,     0,     0,     0,     0,    57,    58,
       0,     0,     0,    60,     7,     8,     9,    10,    26,    93,
       0,    12
};

#define yypact_value_is_default(yystate) \
  ((yystate) == (-21))

#define yytable_value_is_error(yytable_value) \
  YYID (0)

static const yytype_int8 yycheck[] =
{
       0,    21,     2,     3,     4,     1,     6,     7,     8,     9,
      10,     1,     3,    13,    14,    15,    16,    26,    22,    17,
      18,    31,    32,     0,    26,    34,    36,    25,    26,    25,
      24,    34,    42,    31,    32,    35,    36,    26,    36,    39,
      40,    34,    24,    63,    42,    34,    35,    47,    48,    49,
      50,    51,    52,    53,    54,    26,    46,    57,     4,    26,
      60,    -1,    -1,     9,    10,    -1,    12,    13,    14,    -1,
      16,    17,    18,    -1,    64,    -1,    25,    26,    -1,    25,
      26,    -1,    31,    32,    -1,    31,    32,    36,    -1,    -1,
      36,    -1,    -1,    42,    94,    95,    42,     3,    -1,     5,
      -1,     7,    -1,     9,    10,    20,    21,    22,    23,    24,
      -1,    -1,    27,    19,    20,    21,    22,    23,    24,    25,
      26,    27,     3,    -1,     5,    -1,     7,    33,     9,    10,
      36,    -1,    38,    -1,    40,    -1,    -1,    -1,    19,    20,
      21,    22,    23,    24,    -1,    -1,    27,     3,    -1,     5,
      -1,     7,    33,     9,    10,    36,    -1,    38,    -1,    40,
      -1,    -1,    -1,    19,    20,    21,    22,    23,    24,    -1,
       3,    27,     5,    -1,     7,    -1,    -1,    33,    -1,    -1,
      36,    -1,    38,    -1,    40,    -1,    19,    20,    21,    22,
      23,    -1,    -1,     3,    27,     5,    -1,     7,    -1,    -1,
      33,    34,    -1,    36,    -1,    38,    -1,    40,    -1,    19,
      20,    21,    22,    23,    25,    26,    -1,    27,    -1,    -1,
      31,    32,    -1,    33,    -1,    36,    36,    -1,    38,     6,
      40,    42,     9,    10,    -1,    12,    13,    14,    -1,    16,
      17,    18,    -1,    -1,    -1,    25,    26,    -1,    25,    26,
      -1,    31,    32,    -1,    31,    32,    36,    -1,    -1,    36,
      -1,    -1,    42,     9,    10,    42,    12,    13,    14,    -1,
      16,    17,    18,    -1,    -1,    -1,    -1,    -1,    -1,    25,
      26,    -1,    -1,    -1,    -1,    31,    32,    -1,    -1,    35,
      36,    -1,    -1,    -1,     9,    10,    42,    12,    13,    14,
      -1,    16,    17,    18,    -1,    -1,    -1,    -1,    -1,    -1,
      25,    26,    -1,    -1,    -1,    -1,    31,    32,    33,    -1,
      -1,    36,    -1,    -1,    -1,     9,    10,    42,    12,    13,
      14,    -1,    16,    17,    18,    -1,    -1,    -1,    -1,    -1,
      -1,    25,    26,    -1,    -1,    -1,    -1,    31,    32,    -1,
      -1,    -1,    36,    37,    -1,    -1,     9,    10,    42,    12,
      13,    14,    -1,    16,    17,    18,    -1,    -1,    -1,    -1,
      -1,    -1,    25,    26,    -1,    -1,    -1,    -1,    31,    32,
      -1,    -1,    -1,    36,    -1,    -1,    39,     9,    10,    42,
      12,    13,    14,    -1,    16,    17,    18,    -1,    -1,    -1,
      -1,    -1,    -1,    25,    26,    -1,    -1,    -1,    -1,    31,
      32,    -1,    -1,    -1,    36,    -1,    -1,    -1,    -1,    41,
      42,     9,    10,    -1,    12,    13,    14,    -1,    16,    17,
      18,    -1,    -1,    -1,    -1,    -1,    -1,    25,    26,    -1,
      -1,    -1,    -1,    31,    32,    -1,    -1,    -1,    36,    37,
      -1,    -1,     9,    10,    42,    12,    13,    14,    -1,    16,
      17,    18,    -1,    -1,    -1,    -1,    -1,    -1,    25,    26,
      -1,    -1,    -1,    -1,    31,    32,    -1,    -1,    -1,    36,
      37,    -1,    -1,     9,    10,    42,    12,    13,    14,    -1,
      16,    17,    18,    -1,    -1,    -1,    -1,    -1,    -1,    25,
      26,    -1,    -1,    -1,    -1,    31,    32,    -1,    -1,    -1,
      36,    37,    -1,    -1,     9,    10,    42,    12,    13,    14,
      -1,    16,    17,    18,    -1,    -1,    -1,    -1,    -1,    -1,
      25,    26,    -1,    -1,    -1,    -1,    31,    32,    12,    13,
      14,    36,    16,    17,    18,    -1,    -1,    42,    -1,    -1,
      -1,    25,    26,    -1,    -1,    -1,    -1,    31,    32,    12,
      13,    14,    36,    16,    17,    18,    -1,    -1,    42,    -1,
      -1,    -1,    25,    26,    -1,    -1,    -1,    -1,    31,    32,
      -1,    -1,    -1,    36,    20,    21,    22,    23,    24,    42,
      -1,    27
};

/* YYSTOS[STATE-NUM] -- The (internal number of the) accessing
   symbol of state STATE-NUM.  */
static const yytype_uint8 yystos[] =
{
       0,     3,     5,     7,     9,    10,    19,    20,    21,    22,
      23,    24,    27,    33,    36,    38,    40,    44,    45,    46,
      47,    48,    24,    45,    47,     3,    24,    46,    46,    46,
       3,    46,    46,    46,    46,    25,    26,    46,    26,    35,
      22,    46,    46,    46,    46,     0,    34,     9,    10,    12,
      13,    14,    16,    17,    18,    25,    26,    31,    32,    35,
      36,    42,    24,    48,    34,    26,     4,     6,    46,    46,
      46,    46,    46,    33,    37,    39,    41,    24,    45,    46,
      46,    46,    46,    46,    46,    46,    46,    46,    46,    26,
      45,    26,    42,    42,    36,    36,    37,    46,    46,    37,
      37
};

#define yyerrok		(yyerrstatus = 0)
#define yyclearin	(yychar = YYEMPTY)
#define YYEMPTY		(-2)
#define YYEOF		0

#define YYACCEPT	goto yyacceptlab
#define YYABORT		goto yyabortlab
#define YYERROR		goto yyerrorlab


/* Like YYERROR except do call yyerror.  This remains here temporarily
   to ease the transition to the new meaning of YYERROR, for GCC.
   Once GCC version 2 has supplanted version 1, this can go.  However,
   YYFAIL appears to be in use.  Nevertheless, it is formally deprecated
   in Bison 2.4.2's NEWS entry, where a plan to phase it out is
   discussed.  */

#define YYFAIL		goto yyerrlab
#if defined YYFAIL
  /* This is here to suppress warnings from the GCC cpp's
     -Wunused-macros.  Normally we don't worry about that warning, but
     some users do, and we want to make it easy for users to remove
     YYFAIL uses, which will produce warnings from Bison 2.5.  */
#endif

#define YYRECOVERING()  (!!yyerrstatus)

#define YYBACKUP(Token, Value)					\
do								\
  if (yychar == YYEMPTY && yylen == 1)				\
    {								\
      yychar = (Token);						\
      yylval = (Value);						\
      YYPOPSTACK (1);						\
      goto yybackup;						\
    }								\
  else								\
    {								\
      yyerror (&yylloc, yyscanner, YY_("syntax error: cannot back up")); \
      YYERROR;							\
    }								\
while (YYID (0))


#define YYTERROR	1
#define YYERRCODE	256


/* YYLLOC_DEFAULT -- Set CURRENT to span from RHS[1] to RHS[N].
   If N is 0, then set CURRENT to the empty location which ends
   the previous symbol: RHS[0] (always defined).  */

#define YYRHSLOC(Rhs, K) ((Rhs)[K])
#ifndef YYLLOC_DEFAULT
# define YYLLOC_DEFAULT(Current, Rhs, N)				\
    do									\
      if (YYID (N))                                                    \
	{								\
	  (Current).first_line   = YYRHSLOC (Rhs, 1).first_line;	\
	  (Current).first_column = YYRHSLOC (Rhs, 1).first_column;	\
	  (Current).last_line    = YYRHSLOC (Rhs, N).last_line;		\
	  (Current).last_column  = YYRHSLOC (Rhs, N).last_column;	\
	}								\
      else								\
	{								\
	  (Current).first_line   = (Current).last_line   =		\
	    YYRHSLOC (Rhs, 0).last_line;				\
	  (Current).first_column = (Current).last_column =		\
	    YYRHSLOC (Rhs, 0).last_column;				\
	}								\
    while (YYID (0))
#endif


/* YY_LOCATION_PRINT -- Print the location on the stream.
   This macro was not mandated originally: define only if we know
   we won't break user code: when these are the locations we know.  */

#ifndef YY_LOCATION_PRINT
# if defined YYLTYPE_IS_TRIVIAL && YYLTYPE_IS_TRIVIAL
#  define YY_LOCATION_PRINT(File, Loc)			\
     fprintf (File, "%d.%d-%d.%d",			\
	      (Loc).first_line, (Loc).first_column,	\
	      (Loc).last_line,  (Loc).last_column)
# else
#  define YY_LOCATION_PRINT(File, Loc) ((void) 0)
# endif
#endif


/* YYLEX -- calling `yylex' with the right arguments.  */

#ifdef YYLEX_PARAM
# define YYLEX yylex (&yylval, &yylloc, YYLEX_PARAM)
#else
# define YYLEX yylex (&yylval, &yylloc, yyscanner)
#endif

/* Enable debugging if requested.  */
#if YYDEBUG

# ifndef YYFPRINTF
#  include <stdio.h> /* INFRINGES ON USER NAME SPACE */
#  define YYFPRINTF fprintf
# endif

# define YYDPRINTF(Args)			\
do {						\
  if (yydebug)					\
    YYFPRINTF Args;				\
} while (YYID (0))

# define YY_SYMBOL_PRINT(Title, Type, Value, Location)			  \
do {									  \
  if (yydebug)								  \
    {									  \
      YYFPRINTF (stderr, "%s ", Title);					  \
      yy_symbol_print (stderr,						  \
		  Type, Value, Location, yyscanner); \
      YYFPRINTF (stderr, "\n");						  \
    }									  \
} while (YYID (0))


/*--------------------------------.
| Print this symbol on YYOUTPUT.  |
`--------------------------------*/

/*ARGSUSED*/
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_symbol_value_print (FILE *yyoutput, int yytype, YYSTYPE const * const yyvaluep, YYLTYPE const * const yylocationp, yyscan_t yyscanner)
#else
static void
yy_symbol_value_print (yyoutput, yytype, yyvaluep, yylocationp, yyscanner)
    FILE *yyoutput;
    int yytype;
    YYSTYPE const * const yyvaluep;
    YYLTYPE const * const yylocationp;
    yyscan_t yyscanner;
#endif
{
  if (!yyvaluep)
    return;
  YYUSE (yylocationp);
  YYUSE (yyscanner);
# ifdef YYPRINT
  if (yytype < YYNTOKENS)
    YYPRINT (yyoutput, yytoknum[yytype], *yyvaluep);
# else
  YYUSE (yyoutput);
# endif
  switch (yytype)
    {
      default:
	break;
    }
}


/*--------------------------------.
| Print this symbol on YYOUTPUT.  |
`--------------------------------*/

#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_symbol_print (FILE *yyoutput, int yytype, YYSTYPE const * const yyvaluep, YYLTYPE const * const yylocationp, yyscan_t yyscanner)
#else
static void
yy_symbol_print (yyoutput, yytype, yyvaluep, yylocationp, yyscanner)
    FILE *yyoutput;
    int yytype;
    YYSTYPE const * const yyvaluep;
    YYLTYPE const * const yylocationp;
    yyscan_t yyscanner;
#endif
{
  if (yytype < YYNTOKENS)
    YYFPRINTF (yyoutput, "token %s (", yytname[yytype]);
  else
    YYFPRINTF (yyoutput, "nterm %s (", yytname[yytype]);

  YY_LOCATION_PRINT (yyoutput, *yylocationp);
  YYFPRINTF (yyoutput, ": ");
  yy_symbol_value_print (yyoutput, yytype, yyvaluep, yylocationp, yyscanner);
  YYFPRINTF (yyoutput, ")");
}

/*------------------------------------------------------------------.
| yy_stack_print -- Print the state stack from its BOTTOM up to its |
| TOP (included).                                                   |
`------------------------------------------------------------------*/

#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_stack_print (yytype_int16 *yybottom, yytype_int16 *yytop)
#else
static void
yy_stack_print (yybottom, yytop)
    yytype_int16 *yybottom;
    yytype_int16 *yytop;
#endif
{
  YYFPRINTF (stderr, "Stack now");
  for (; yybottom <= yytop; yybottom++)
    {
      int yybot = *yybottom;
      YYFPRINTF (stderr, " %d", yybot);
    }
  YYFPRINTF (stderr, "\n");
}

# define YY_STACK_PRINT(Bottom, Top)				\
do {								\
  if (yydebug)							\
    yy_stack_print ((Bottom), (Top));				\
} while (YYID (0))


/*------------------------------------------------.
| Report that the YYRULE is going to be reduced.  |
`------------------------------------------------*/

#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_reduce_print (YYSTYPE *yyvsp, YYLTYPE *yylsp, int yyrule, yyscan_t yyscanner)
#else
static void
yy_reduce_print (yyvsp, yylsp, yyrule, yyscanner)
    YYSTYPE *yyvsp;
    YYLTYPE *yylsp;
    int yyrule;
    yyscan_t yyscanner;
#endif
{
  int yynrhs = yyr2[yyrule];
  int yyi;
  unsigned long int yylno = yyrline[yyrule];
  YYFPRINTF (stderr, "Reducing stack by rule %d (line %lu):\n",
	     yyrule - 1, yylno);
  /* The symbols being reduced.  */
  for (yyi = 0; yyi < yynrhs; yyi++)
    {
      YYFPRINTF (stderr, "   $%d = ", yyi + 1);
      yy_symbol_print (stderr, yyrhs[yyprhs[yyrule] + yyi],
		       &(yyvsp[(yyi + 1) - (yynrhs)])
		       , &(yylsp[(yyi + 1) - (yynrhs)])		       , yyscanner);
      YYFPRINTF (stderr, "\n");
    }
}

# define YY_REDUCE_PRINT(Rule)		\
do {					\
  if (yydebug)				\
    yy_reduce_print (yyvsp, yylsp, Rule, yyscanner); \
} while (YYID (0))

/* Nonzero means print parse trace.  It is left uninitialized so that
   multiple parsers can coexist.  */
int yydebug;
#else /* !YYDEBUG */
# define YYDPRINTF(Args)
# define YY_SYMBOL_PRINT(Title, Type, Value, Location)
# define YY_STACK_PRINT(Bottom, Top)
# define YY_REDUCE_PRINT(Rule)
#endif /* !YYDEBUG */


/* YYINITDEPTH -- initial size of the parser's stacks.  */
#ifndef	YYINITDEPTH
# define YYINITDEPTH 200
#endif

/* YYMAXDEPTH -- maximum size the stacks can grow to (effective only
   if the built-in stack extension method is used).

   Do not make this value too large; the results are undefined if
   YYSTACK_ALLOC_MAXIMUM < YYSTACK_BYTES (YYMAXDEPTH)
   evaluated with infinite-precision integer arithmetic.  */

#ifndef YYMAXDEPTH
# define YYMAXDEPTH 10000
#endif


#if YYERROR_VERBOSE

# ifndef yystrlen
#  if defined __GLIBC__ && defined _STRING_H
#   define yystrlen strlen
#  else
/* Return the length of YYSTR.  */
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static YYSIZE_T
yystrlen (const char *yystr)
#else
static YYSIZE_T
yystrlen (yystr)
    const char *yystr;
#endif
{
  YYSIZE_T yylen;
  for (yylen = 0; yystr[yylen]; yylen++)
    continue;
  return yylen;
}
#  endif
# endif

# ifndef yystpcpy
#  if defined __GLIBC__ && defined _STRING_H && defined _GNU_SOURCE
#   define yystpcpy stpcpy
#  else
/* Copy YYSRC to YYDEST, returning the address of the terminating '\0' in
   YYDEST.  */
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static char *
yystpcpy (char *yydest, const char *yysrc)
#else
static char *
yystpcpy (yydest, yysrc)
    char *yydest;
    const char *yysrc;
#endif
{
  char *yyd = yydest;
  const char *yys = yysrc;

  while ((*yyd++ = *yys++) != '\0')
    continue;

  return yyd - 1;
}
#  endif
# endif

# ifndef yytnamerr
/* Copy to YYRES the contents of YYSTR after stripping away unnecessary
   quotes and backslashes, so that it's suitable for yyerror.  The
   heuristic is that double-quoting is unnecessary unless the string
   contains an apostrophe, a comma, or backslash (other than
   backslash-backslash).  YYSTR is taken from yytname.  If YYRES is
   null, do not copy; instead, return the length of what the result
   would have been.  */
static YYSIZE_T
yytnamerr (char *yyres, const char *yystr)
{
  if (*yystr == '"')
    {
      YYSIZE_T yyn = 0;
      char const *yyp = yystr;

      for (;;)
	switch (*++yyp)
	  {
	  case '\'':
	  case ',':
	    goto do_not_strip_quotes;

	  case '\\':
	    if (*++yyp != '\\')
	      goto do_not_strip_quotes;
	    /* Fall through.  */
	  default:
	    if (yyres)
	      yyres[yyn] = *yyp;
	    yyn++;
	    break;

	  case '"':
	    if (yyres)
	      yyres[yyn] = '\0';
	    return yyn;
	  }
    do_not_strip_quotes: ;
    }

  if (! yyres)
    return yystrlen (yystr);

  return yystpcpy (yyres, yystr) - yyres;
}
# endif

/* Copy into *YYMSG, which is of size *YYMSG_ALLOC, an error message
   about the unexpected token YYTOKEN for the state stack whose top is
   YYSSP.

   Return 0 if *YYMSG was successfully written.  Return 1 if *YYMSG is
   not large enough to hold the message.  In that case, also set
   *YYMSG_ALLOC to the required number of bytes.  Return 2 if the
   required number of bytes is too large to store.  */
static int
yysyntax_error (YYSIZE_T *yymsg_alloc, char **yymsg,
                yytype_int16 *yyssp, int yytoken)
{
  YYSIZE_T yysize0 = yytnamerr (0, yytname[yytoken]);
  YYSIZE_T yysize = yysize0;
  YYSIZE_T yysize1;
  enum { YYERROR_VERBOSE_ARGS_MAXIMUM = 5 };
  /* Internationalized format string. */
  const char *yyformat = 0;
  /* Arguments of yyformat. */
  char const *yyarg[YYERROR_VERBOSE_ARGS_MAXIMUM];
  /* Number of reported tokens (one for the "unexpected", one per
     "expected"). */
  int yycount = 0;

  /* There are many possibilities here to consider:
     - Assume YYFAIL is not used.  It's too flawed to consider.  See
       <http://lists.gnu.org/archive/html/bison-patches/2009-12/msg00024.html>
       for details.  YYERROR is fine as it does not invoke this
       function.
     - If this state is a consistent state with a default action, then
       the only way this function was invoked is if the default action
       is an error action.  In that case, don't check for expected
       tokens because there are none.
     - The only way there can be no lookahead present (in yychar) is if
       this state is a consistent state with a default action.  Thus,
       detecting the absence of a lookahead is sufficient to determine
       that there is no unexpected or expected token to report.  In that
       case, just report a simple "syntax error".
     - Don't assume there isn't a lookahead just because this state is a
       consistent state with a default action.  There might have been a
       previous inconsistent state, consistent state with a non-default
       action, or user semantic action that manipulated yychar.
     - Of course, the expected token list depends on states to have
       correct lookahead information, and it depends on the parser not
       to perform extra reductions after fetching a lookahead from the
       scanner and before detecting a syntax error.  Thus, state merging
       (from LALR or IELR) and default reductions corrupt the expected
       token list.  However, the list is correct for canonical LR with
       one exception: it will still contain any token that will not be
       accepted due to an error action in a later state.
  */
  if (yytoken != YYEMPTY)
    {
      int yyn = yypact[*yyssp];
      yyarg[yycount++] = yytname[yytoken];
      if (!yypact_value_is_default (yyn))
        {
          /* Start YYX at -YYN if negative to avoid negative indexes in
             YYCHECK.  In other words, skip the first -YYN actions for
             this state because they are default actions.  */
          int yyxbegin = yyn < 0 ? -yyn : 0;
          /* Stay within bounds of both yycheck and yytname.  */
          int yychecklim = YYLAST - yyn + 1;
          int yyxend = yychecklim < YYNTOKENS ? yychecklim : YYNTOKENS;
          int yyx;

          for (yyx = yyxbegin; yyx < yyxend; ++yyx)
            if (yycheck[yyx + yyn] == yyx && yyx != YYTERROR
                && !yytable_value_is_error (yytable[yyx + yyn]))
              {
                if (yycount == YYERROR_VERBOSE_ARGS_MAXIMUM)
                  {
                    yycount = 1;
                    yysize = yysize0;
                    break;
                  }
                yyarg[yycount++] = yytname[yyx];
                yysize1 = yysize + yytnamerr (0, yytname[yyx]);
                if (! (yysize <= yysize1
                       && yysize1 <= YYSTACK_ALLOC_MAXIMUM))
                  return 2;
                yysize = yysize1;
              }
        }
    }

  switch (yycount)
    {
# define YYCASE_(N, S)                      \
      case N:                               \
        yyformat = S;                       \
      break
      YYCASE_(0, YY_("syntax error"));
      YYCASE_(1, YY_("syntax error, unexpected %s"));
      YYCASE_(2, YY_("syntax error, unexpected %s, expecting %s"));
      YYCASE_(3, YY_("syntax error, unexpected %s, expecting %s or %s"));
      YYCASE_(4, YY_("syntax error, unexpected %s, expecting %s or %s or %s"));
      YYCASE_(5, YY_("syntax error, unexpected %s, expecting %s or %s or %s or %s"));
# undef YYCASE_
    }

  yysize1 = yysize + yystrlen (yyformat);
  if (! (yysize <= yysize1 && yysize1 <= YYSTACK_ALLOC_MAXIMUM))
    return 2;
  yysize = yysize1;

  if (*yymsg_alloc < yysize)
    {
      *yymsg_alloc = 2 * yysize;
      if (! (yysize <= *yymsg_alloc
             && *yymsg_alloc <= YYSTACK_ALLOC_MAXIMUM))
        *yymsg_alloc = YYSTACK_ALLOC_MAXIMUM;
      return 1;
    }

  /* Avoid sprintf, as that infringes on the user's name space.
     Don't have undefined behavior even if the translation
     produced a string with the wrong number of "%s"s.  */
  {
    char *yyp = *yymsg;
    int yyi = 0;
    while ((*yyp = *yyformat) != '\0')
      if (*yyp == '%' && yyformat[1] == 's' && yyi < yycount)
        {
          yyp += yytnamerr (yyp, yyarg[yyi++]);
          yyformat += 2;
        }
      else
        {
          yyp++;
          yyformat++;
        }
  }
  return 0;
}
#endif /* YYERROR_VERBOSE */

/*-----------------------------------------------.
| Release the memory associated to this symbol.  |
`-----------------------------------------------*/

/*ARGSUSED*/
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yydestruct (const char *yymsg, int yytype, YYSTYPE *yyvaluep, YYLTYPE *yylocationp, yyscan_t yyscanner)
#else
static void
yydestruct (yymsg, yytype, yyvaluep, yylocationp, yyscanner)
    const char *yymsg;
    int yytype;
    YYSTYPE *yyvaluep;
    YYLTYPE *yylocationp;
    yyscan_t yyscanner;
#endif
{
  YYUSE (yyvaluep);
  YYUSE (yylocationp);
  YYUSE (yyscanner);

  if (!yymsg)
    yymsg = "Deleting";
  YY_SYMBOL_PRINT (yymsg, yytype, yyvaluep, yylocationp);

  switch (yytype)
    {

      default:
	break;
    }
}


/* Prevent warnings from -Wmissing-prototypes.  */
#ifdef YYPARSE_PARAM
#if defined __STDC__ || defined __cplusplus
int yyparse (void *YYPARSE_PARAM);
#else
int yyparse ();
#endif
#else /* ! YYPARSE_PARAM */
#if defined __STDC__ || defined __cplusplus
int yyparse (yyscan_t yyscanner);
#else
int yyparse ();
#endif
#endif /* ! YYPARSE_PARAM */


/*----------.
| yyparse.  |
`----------*/

#ifdef YYPARSE_PARAM
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
int
yyparse (void *YYPARSE_PARAM)
#else
int
yyparse (YYPARSE_PARAM)
    void *YYPARSE_PARAM;
#endif
#else /* ! YYPARSE_PARAM */
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
int
yyparse (yyscan_t yyscanner)
#else
int
yyparse (yyscanner)
    yyscan_t yyscanner;
#endif
#endif
{
/* The lookahead symbol.  */
int yychar;

/* The semantic value of the lookahead symbol.  */
YYSTYPE yylval;

/* Location data for the lookahead symbol.  */
YYLTYPE yylloc;

    /* Number of syntax errors so far.  */
    int yynerrs;

    int yystate;
    /* Number of tokens to shift before error messages enabled.  */
    int yyerrstatus;

    /* The stacks and their tools:
       `yyss': related to states.
       `yyvs': related to semantic values.
       `yyls': related to locations.

       Refer to the stacks thru separate pointers, to allow yyoverflow
       to reallocate them elsewhere.  */

    /* The state stack.  */
    yytype_int16 yyssa[YYINITDEPTH];
    yytype_int16 *yyss;
    yytype_int16 *yyssp;

    /* The semantic value stack.  */
    YYSTYPE yyvsa[YYINITDEPTH];
    YYSTYPE *yyvs;
    YYSTYPE *yyvsp;

    /* The location stack.  */
    YYLTYPE yylsa[YYINITDEPTH];
    YYLTYPE *yyls;
    YYLTYPE *yylsp;

    /* The locations where the error started and ended.  */
    YYLTYPE yyerror_range[3];

    YYSIZE_T yystacksize;

  int yyn;
  int yyresult;
  /* Lookahead token as an internal (translated) token number.  */
  int yytoken;
  /* The variables used to return semantic value and location from the
     action routines.  */
  YYSTYPE yyval;
  YYLTYPE yyloc;

#if YYERROR_VERBOSE
  /* Buffer for error messages, and its allocated size.  */
  char yymsgbuf[128];
  char *yymsg = yymsgbuf;
  YYSIZE_T yymsg_alloc = sizeof yymsgbuf;
#endif

#define YYPOPSTACK(N)   (yyvsp -= (N), yyssp -= (N), yylsp -= (N))

  /* The number of symbols on the RHS of the reduced rule.
     Keep to zero when no symbol should be popped.  */
  int yylen = 0;

  yytoken = 0;
  yyss = yyssa;
  yyvs = yyvsa;
  yyls = yylsa;
  yystacksize = YYINITDEPTH;

  YYDPRINTF ((stderr, "Starting parse\n"));

  yystate = 0;
  yyerrstatus = 0;
  yynerrs = 0;
  yychar = YYEMPTY; /* Cause a token to be read.  */

  /* Initialize stack pointers.
     Waste one element of value and location stack
     so that they stay on the same level as the state stack.
     The wasted elements are never initialized.  */
  yyssp = yyss;
  yyvsp = yyvs;
  yylsp = yyls;

#if defined YYLTYPE_IS_TRIVIAL && YYLTYPE_IS_TRIVIAL
  /* Initialize the default location before parsing starts.  */
  yylloc.first_line   = yylloc.last_line   = 1;
  yylloc.first_column = yylloc.last_column = 1;
#endif

  goto yysetstate;

/*------------------------------------------------------------.
| yynewstate -- Push a new state, which is found in yystate.  |
`------------------------------------------------------------*/
 yynewstate:
  /* In all cases, when you get here, the value and location stacks
     have just been pushed.  So pushing a state here evens the stacks.  */
  yyssp++;

 yysetstate:
  *yyssp = yystate;

  if (yyss + yystacksize - 1 <= yyssp)
    {
      /* Get the current used size of the three stacks, in elements.  */
      YYSIZE_T yysize = yyssp - yyss + 1;

#ifdef yyoverflow
      {
	/* Give user a chance to reallocate the stack.  Use copies of
	   these so that the &'s don't force the real ones into
	   memory.  */
	YYSTYPE *yyvs1 = yyvs;
	yytype_int16 *yyss1 = yyss;
	YYLTYPE *yyls1 = yyls;

	/* Each stack pointer address is followed by the size of the
	   data in use in that stack, in bytes.  This used to be a
	   conditional around just the two extra args, but that might
	   be undefined if yyoverflow is a macro.  */
	yyoverflow (YY_("memory exhausted"),
		    &yyss1, yysize * sizeof (*yyssp),
		    &yyvs1, yysize * sizeof (*yyvsp),
		    &yyls1, yysize * sizeof (*yylsp),
		    &yystacksize);

	yyls = yyls1;
	yyss = yyss1;
	yyvs = yyvs1;
      }
#else /* no yyoverflow */
# ifndef YYSTACK_RELOCATE
      goto yyexhaustedlab;
# else
      /* Extend the stack our own way.  */
      if (YYMAXDEPTH <= yystacksize)
	goto yyexhaustedlab;
      yystacksize *= 2;
      if (YYMAXDEPTH < yystacksize)
	yystacksize = YYMAXDEPTH;

      {
	yytype_int16 *yyss1 = yyss;
	union yyalloc *yyptr =
	  (union yyalloc *) YYSTACK_ALLOC (YYSTACK_BYTES (yystacksize));
	if (! yyptr)
	  goto yyexhaustedlab;
	YYSTACK_RELOCATE (yyss_alloc, yyss);
	YYSTACK_RELOCATE (yyvs_alloc, yyvs);
	YYSTACK_RELOCATE (yyls_alloc, yyls);
#  undef YYSTACK_RELOCATE
	if (yyss1 != yyssa)
	  YYSTACK_FREE (yyss1);
      }
# endif
#endif /* no yyoverflow */

      yyssp = yyss + yysize - 1;
      yyvsp = yyvs + yysize - 1;
      yylsp = yyls + yysize - 1;

      YYDPRINTF ((stderr, "Stack size increased to %lu\n",
		  (unsigned long int) yystacksize));

      if (yyss + yystacksize - 1 <= yyssp)
	YYABORT;
    }

  YYDPRINTF ((stderr, "Entering state %d\n", yystate));

  if (yystate == YYFINAL)
    YYACCEPT;

  goto yybackup;

/*-----------.
| yybackup.  |
`-----------*/
yybackup:

  /* Do appropriate processing given the current state.  Read a
     lookahead token if we need one and don't already have one.  */

  /* First try to decide what to do without reference to lookahead token.  */
  yyn = yypact[yystate];
  if (yypact_value_is_default (yyn))
    goto yydefault;

  /* Not known => get a lookahead token if don't already have one.  */

  /* YYCHAR is either YYEMPTY or YYEOF or a valid lookahead symbol.  */
  if (yychar == YYEMPTY)
    {
      YYDPRINTF ((stderr, "Reading a token: "));
      yychar = YYLEX;
    }

  if (yychar <= YYEOF)
    {
      yychar = yytoken = YYEOF;
      YYDPRINTF ((stderr, "Now at end of input.\n"));
    }
  else
    {
      yytoken = YYTRANSLATE (yychar);
      YY_SYMBOL_PRINT ("Next token is", yytoken, &yylval, &yylloc);
    }

  /* If the proper action on seeing token YYTOKEN is to reduce or to
     detect an error, take that action.  */
  yyn += yytoken;
  if (yyn < 0 || YYLAST < yyn || yycheck[yyn] != yytoken)
    goto yydefault;
  yyn = yytable[yyn];
  if (yyn <= 0)
    {
      if (yytable_value_is_error (yyn))
        goto yyerrlab;
      yyn = -yyn;
      goto yyreduce;
    }

  /* Count tokens shifted since error; after three, turn off error
     status.  */
  if (yyerrstatus)
    yyerrstatus--;

  /* Shift the lookahead token.  */
  YY_SYMBOL_PRINT ("Shifting", yytoken, &yylval, &yylloc);

  /* Discard the shifted token.  */
  yychar = YYEMPTY;

  yystate = yyn;
  *++yyvsp = yylval;
  *++yylsp = yylloc;
  goto yynewstate;


/*-----------------------------------------------------------.
| yydefault -- do the default action for the current state.  |
`-----------------------------------------------------------*/
yydefault:
  yyn = yydefact[yystate];
  if (yyn == 0)
    goto yyerrlab;
  goto yyreduce;


/*-----------------------------.
| yyreduce -- Do a reduction.  |
`-----------------------------*/
yyreduce:
  /* yyn is the number of a rule to reduce with.  */
  yylen = yyr2[yyn];

  /* If YYLEN is nonzero, implement the default value of the action:
     `$$ = $1'.

     Otherwise, the following line sets YYVAL to garbage.
     This behavior is undocumented and Bison
     users should not rely upon it.  Assigning to YYVAL
     unconditionally makes the parser a bit smaller, and it avoids a
     GCC warning that YYVAL may be used uninitialized.  */
  yyval = yyvsp[1-yylen];

  /* Default location.  */
  YYLLOC_DEFAULT (yyloc, (yylsp - yylen), yylen);
  YY_REDUCE_PRINT (yyn);
  switch (yyn)
    {
        case 2:

/* Line 1806 of yacc.c  */
#line 196 "./mp-equation-parser.y"
    { set_result(yyscanner, &(yyvsp[(1) - (1)].int_t)); }
    break;

  case 3:

/* Line 1806 of yacc.c  */
#line 197 "./mp-equation-parser.y"
    { set_result(yyscanner, &(yyvsp[(1) - (2)].int_t)); }
    break;

  case 4:

/* Line 1806 of yacc.c  */
#line 198 "./mp-equation-parser.y"
    {set_variable(yyscanner, (yyvsp[(1) - (3)].name), &(yyvsp[(3) - (3)].int_t)); set_result(yyscanner, &(yyvsp[(3) - (3)].int_t)); }
    break;

  case 5:

/* Line 1806 of yacc.c  */
#line 199 "./mp-equation-parser.y"
    { MPNumber t; do_conversion(yyscanner, &(yyvsp[(1) - (4)].int_t), (yyvsp[(2) - (4)].name), (yyvsp[(4) - (4)].name), &t); set_result(yyscanner, &t); free((yyvsp[(2) - (4)].name)); free((yyvsp[(4) - (4)].name)); }
    break;

  case 6:

/* Line 1806 of yacc.c  */
#line 200 "./mp-equation-parser.y"
    { MPNumber x, t; mp_set_from_integer(1, &x); do_conversion(yyscanner, &x, (yyvsp[(1) - (3)].name), (yyvsp[(3) - (3)].name), &t); set_result(yyscanner, &t); free((yyvsp[(1) - (3)].name)); free((yyvsp[(3) - (3)].name)); }
    break;

  case 7:

/* Line 1806 of yacc.c  */
#line 204 "./mp-equation-parser.y"
    {(yyval.name) = (yyvsp[(1) - (1)].name);}
    break;

  case 8:

/* Line 1806 of yacc.c  */
#line 205 "./mp-equation-parser.y"
    {(yyval.name) = make_unit((yyvsp[(1) - (2)].name), (yyvsp[(2) - (2)].integer)); free((yyvsp[(1) - (2)].name));}
    break;

  case 9:

/* Line 1806 of yacc.c  */
#line 210 "./mp-equation-parser.y"
    {mp_set_from_mp(&(yyvsp[(2) - (3)].int_t), &(yyval.int_t));}
    break;

  case 10:

/* Line 1806 of yacc.c  */
#line 211 "./mp-equation-parser.y"
    {mp_divide(&(yyvsp[(1) - (6)].int_t), &(yyvsp[(3) - (6)].int_t), &(yyval.int_t)); mp_multiply(&(yyvsp[(5) - (6)].int_t), &(yyval.int_t), &(yyval.int_t));}
    break;

  case 11:

/* Line 1806 of yacc.c  */
#line 212 "./mp-equation-parser.y"
    {mp_modulus_divide(&(yyvsp[(1) - (6)].int_t), &(yyvsp[(3) - (6)].int_t), &(yyval.int_t)); mp_multiply(&(yyvsp[(5) - (6)].int_t), &(yyval.int_t), &(yyval.int_t));}
    break;

  case 12:

/* Line 1806 of yacc.c  */
#line 213 "./mp-equation-parser.y"
    {mp_multiply(&(yyvsp[(1) - (4)].int_t), &(yyvsp[(3) - (4)].int_t), &(yyval.int_t));}
    break;

  case 13:

/* Line 1806 of yacc.c  */
#line 214 "./mp-equation-parser.y"
    {mp_floor(&(yyvsp[(2) - (3)].int_t), &(yyval.int_t));}
    break;

  case 14:

/* Line 1806 of yacc.c  */
#line 215 "./mp-equation-parser.y"
    {mp_ceiling(&(yyvsp[(2) - (3)].int_t), &(yyval.int_t));}
    break;

  case 15:

/* Line 1806 of yacc.c  */
#line 216 "./mp-equation-parser.y"
    {mp_round(&(yyvsp[(2) - (3)].int_t), &(yyval.int_t));}
    break;

  case 16:

/* Line 1806 of yacc.c  */
#line 217 "./mp-equation-parser.y"
    {mp_fractional_part(&(yyvsp[(2) - (3)].int_t), &(yyval.int_t));}
    break;

  case 17:

/* Line 1806 of yacc.c  */
#line 218 "./mp-equation-parser.y"
    {mp_abs(&(yyvsp[(2) - (3)].int_t), &(yyval.int_t));}
    break;

  case 18:

/* Line 1806 of yacc.c  */
#line 219 "./mp-equation-parser.y"
    {mp_xpowy(&(yyvsp[(1) - (3)].int_t), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t));}
    break;

  case 19:

/* Line 1806 of yacc.c  */
#line 220 "./mp-equation-parser.y"
    {mp_xpowy_integer(&(yyvsp[(1) - (2)].int_t), (yyvsp[(2) - (2)].integer), &(yyval.int_t));}
    break;

  case 20:

/* Line 1806 of yacc.c  */
#line 221 "./mp-equation-parser.y"
    {mp_xpowy_integer(&(yyvsp[(1) - (2)].int_t), (yyvsp[(2) - (2)].integer), &(yyval.int_t));}
    break;

  case 21:

/* Line 1806 of yacc.c  */
#line 222 "./mp-equation-parser.y"
    {mp_factorial(&(yyvsp[(1) - (2)].int_t), &(yyval.int_t));}
    break;

  case 22:

/* Line 1806 of yacc.c  */
#line 223 "./mp-equation-parser.y"
    {mp_set_from_mp(&(yyvsp[(1) - (1)].int_t), &(yyval.int_t));}
    break;

  case 23:

/* Line 1806 of yacc.c  */
#line 224 "./mp-equation-parser.y"
    {mp_multiply(&(yyvsp[(1) - (2)].int_t), &(yyvsp[(2) - (2)].int_t), &(yyval.int_t));}
    break;

  case 24:

/* Line 1806 of yacc.c  */
#line 225 "./mp-equation-parser.y"
    {mp_invert_sign(&(yyvsp[(2) - (2)].int_t), &(yyval.int_t));}
    break;

  case 25:

/* Line 1806 of yacc.c  */
#line 226 "./mp-equation-parser.y"
    {mp_set_from_mp(&(yyvsp[(2) - (2)].int_t), &(yyval.int_t));}
    break;

  case 26:

/* Line 1806 of yacc.c  */
#line 227 "./mp-equation-parser.y"
    {mp_divide(&(yyvsp[(1) - (3)].int_t), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t));}
    break;

  case 27:

/* Line 1806 of yacc.c  */
#line 228 "./mp-equation-parser.y"
    {mp_modulus_divide(&(yyvsp[(1) - (3)].int_t), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t));}
    break;

  case 28:

/* Line 1806 of yacc.c  */
#line 229 "./mp-equation-parser.y"
    {mp_multiply(&(yyvsp[(1) - (3)].int_t), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t));}
    break;

  case 29:

/* Line 1806 of yacc.c  */
#line 230 "./mp-equation-parser.y"
    {mp_add_integer(&(yyvsp[(3) - (4)].int_t), 100, &(yyvsp[(3) - (4)].int_t)); mp_divide_integer(&(yyvsp[(3) - (4)].int_t), 100, &(yyvsp[(3) - (4)].int_t)); mp_multiply(&(yyvsp[(1) - (4)].int_t), &(yyvsp[(3) - (4)].int_t), &(yyval.int_t));}
    break;

  case 30:

/* Line 1806 of yacc.c  */
#line 231 "./mp-equation-parser.y"
    {mp_add_integer(&(yyvsp[(3) - (4)].int_t), -100, &(yyvsp[(3) - (4)].int_t)); mp_divide_integer(&(yyvsp[(3) - (4)].int_t), -100, &(yyvsp[(3) - (4)].int_t)); mp_multiply(&(yyvsp[(1) - (4)].int_t), &(yyvsp[(3) - (4)].int_t), &(yyval.int_t));}
    break;

  case 31:

/* Line 1806 of yacc.c  */
#line 232 "./mp-equation-parser.y"
    {mp_add(&(yyvsp[(1) - (3)].int_t), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t));}
    break;

  case 32:

/* Line 1806 of yacc.c  */
#line 233 "./mp-equation-parser.y"
    {mp_subtract(&(yyvsp[(1) - (3)].int_t), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t));}
    break;

  case 33:

/* Line 1806 of yacc.c  */
#line 234 "./mp-equation-parser.y"
    {mp_divide_integer(&(yyvsp[(1) - (2)].int_t), 100, &(yyval.int_t));}
    break;

  case 34:

/* Line 1806 of yacc.c  */
#line 235 "./mp-equation-parser.y"
    {do_not(yyscanner, &(yyvsp[(2) - (2)].int_t), &(yyval.int_t));}
    break;

  case 35:

/* Line 1806 of yacc.c  */
#line 236 "./mp-equation-parser.y"
    {mp_and(&(yyvsp[(1) - (3)].int_t), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t));}
    break;

  case 36:

/* Line 1806 of yacc.c  */
#line 237 "./mp-equation-parser.y"
    {mp_or(&(yyvsp[(1) - (3)].int_t), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t));}
    break;

  case 37:

/* Line 1806 of yacc.c  */
#line 238 "./mp-equation-parser.y"
    {mp_xor(&(yyvsp[(1) - (3)].int_t), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t));}
    break;

  case 38:

/* Line 1806 of yacc.c  */
#line 239 "./mp-equation-parser.y"
    {mp_set_from_mp(&(yyvsp[(1) - (1)].int_t), &(yyval.int_t));}
    break;

  case 39:

/* Line 1806 of yacc.c  */
#line 244 "./mp-equation-parser.y"
    {mp_set_from_mp(&(yyvsp[(1) - (1)].int_t), &(yyval.int_t));}
    break;

  case 40:

/* Line 1806 of yacc.c  */
#line 245 "./mp-equation-parser.y"
    {if (!get_function(yyscanner, (yyvsp[(1) - (2)].name), &(yyvsp[(2) - (2)].int_t), &(yyval.int_t))) YYABORT; free((yyvsp[(1) - (2)].name));}
    break;

  case 41:

/* Line 1806 of yacc.c  */
#line 246 "./mp-equation-parser.y"
    {if (!get_function(yyscanner, (yyvsp[(1) - (3)].name), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t))) YYABORT; mp_xpowy_integer(&(yyval.int_t), (yyvsp[(2) - (3)].integer), &(yyval.int_t)); free((yyvsp[(1) - (3)].name));}
    break;

  case 42:

/* Line 1806 of yacc.c  */
#line 247 "./mp-equation-parser.y"
    {if (!get_inverse_function(yyscanner, (yyvsp[(1) - (3)].name), &(yyvsp[(3) - (3)].int_t), &(yyval.int_t))) YYABORT; mp_xpowy_integer(&(yyval.int_t), -(yyvsp[(2) - (3)].integer), &(yyval.int_t)); free((yyvsp[(1) - (3)].name));}
    break;

  case 43:

/* Line 1806 of yacc.c  */
#line 248 "./mp-equation-parser.y"
    {set_error(yyscanner, PARSER_ERR_UNKNOWN_FUNCTION, (yyvsp[(1) - (3)].name)); free((yyvsp[(1) - (3)].name)); YYABORT;}
    break;

  case 44:

/* Line 1806 of yacc.c  */
#line 249 "./mp-equation-parser.y"
    {mp_root(&(yyvsp[(3) - (3)].int_t), (yyvsp[(1) - (3)].integer), &(yyval.int_t));}
    break;

  case 45:

/* Line 1806 of yacc.c  */
#line 250 "./mp-equation-parser.y"
    {mp_sqrt(&(yyvsp[(2) - (2)].int_t), &(yyval.int_t));}
    break;

  case 46:

/* Line 1806 of yacc.c  */
#line 251 "./mp-equation-parser.y"
    {mp_root(&(yyvsp[(2) - (2)].int_t), 3, &(yyval.int_t));}
    break;

  case 47:

/* Line 1806 of yacc.c  */
#line 252 "./mp-equation-parser.y"
    {mp_root(&(yyvsp[(2) - (2)].int_t), 4, &(yyval.int_t));}
    break;

  case 48:

/* Line 1806 of yacc.c  */
#line 256 "./mp-equation-parser.y"
    {if (!get_variable(yyscanner, (yyvsp[(1) - (1)].name), 1, &(yyval.int_t))) YYABORT; free((yyvsp[(1) - (1)].name));}
    break;

  case 49:

/* Line 1806 of yacc.c  */
#line 257 "./mp-equation-parser.y"
    {if (!get_variable(yyscanner, (yyvsp[(1) - (2)].name), (yyvsp[(2) - (2)].integer), &(yyval.int_t))) YYABORT; free((yyvsp[(1) - (2)].name));}
    break;

  case 50:

/* Line 1806 of yacc.c  */
#line 258 "./mp-equation-parser.y"
    {mp_multiply(&(yyvsp[(1) - (2)].int_t), &(yyvsp[(2) - (2)].int_t), &(yyval.int_t));}
    break;



/* Line 1806 of yacc.c  */
#line 2171 "mp-equation-parser.c"
      default: break;
    }
  /* User semantic actions sometimes alter yychar, and that requires
     that yytoken be updated with the new translation.  We take the
     approach of translating immediately before every use of yytoken.
     One alternative is translating here after every semantic action,
     but that translation would be missed if the semantic action invokes
     YYABORT, YYACCEPT, or YYERROR immediately after altering yychar or
     if it invokes YYBACKUP.  In the case of YYABORT or YYACCEPT, an
     incorrect destructor might then be invoked immediately.  In the
     case of YYERROR or YYBACKUP, subsequent parser actions might lead
     to an incorrect destructor call or verbose syntax error message
     before the lookahead is translated.  */
  YY_SYMBOL_PRINT ("-> $$ =", yyr1[yyn], &yyval, &yyloc);

  YYPOPSTACK (yylen);
  yylen = 0;
  YY_STACK_PRINT (yyss, yyssp);

  *++yyvsp = yyval;
  *++yylsp = yyloc;

  /* Now `shift' the result of the reduction.  Determine what state
     that goes to, based on the state we popped back to and the rule
     number reduced by.  */

  yyn = yyr1[yyn];

  yystate = yypgoto[yyn - YYNTOKENS] + *yyssp;
  if (0 <= yystate && yystate <= YYLAST && yycheck[yystate] == *yyssp)
    yystate = yytable[yystate];
  else
    yystate = yydefgoto[yyn - YYNTOKENS];

  goto yynewstate;


/*------------------------------------.
| yyerrlab -- here on detecting error |
`------------------------------------*/
yyerrlab:
  /* Make sure we have latest lookahead translation.  See comments at
     user semantic actions for why this is necessary.  */
  yytoken = yychar == YYEMPTY ? YYEMPTY : YYTRANSLATE (yychar);

  /* If not already recovering from an error, report this error.  */
  if (!yyerrstatus)
    {
      ++yynerrs;
#if ! YYERROR_VERBOSE
      yyerror (&yylloc, yyscanner, YY_("syntax error"));
#else
# define YYSYNTAX_ERROR yysyntax_error (&yymsg_alloc, &yymsg, \
                                        yyssp, yytoken)
      {
        char const *yymsgp = YY_("syntax error");
        int yysyntax_error_status;
        yysyntax_error_status = YYSYNTAX_ERROR;
        if (yysyntax_error_status == 0)
          yymsgp = yymsg;
        else if (yysyntax_error_status == 1)
          {
            if (yymsg != yymsgbuf)
              YYSTACK_FREE (yymsg);
            yymsg = (char *) YYSTACK_ALLOC (yymsg_alloc);
            if (!yymsg)
              {
                yymsg = yymsgbuf;
                yymsg_alloc = sizeof yymsgbuf;
                yysyntax_error_status = 2;
              }
            else
              {
                yysyntax_error_status = YYSYNTAX_ERROR;
                yymsgp = yymsg;
              }
          }
        yyerror (&yylloc, yyscanner, yymsgp);
        if (yysyntax_error_status == 2)
          goto yyexhaustedlab;
      }
# undef YYSYNTAX_ERROR
#endif
    }

  yyerror_range[1] = yylloc;

  if (yyerrstatus == 3)
    {
      /* If just tried and failed to reuse lookahead token after an
	 error, discard it.  */

      if (yychar <= YYEOF)
	{
	  /* Return failure if at end of input.  */
	  if (yychar == YYEOF)
	    YYABORT;
	}
      else
	{
	  yydestruct ("Error: discarding",
		      yytoken, &yylval, &yylloc, yyscanner);
	  yychar = YYEMPTY;
	}
    }

  /* Else will try to reuse lookahead token after shifting the error
     token.  */
  goto yyerrlab1;


/*---------------------------------------------------.
| yyerrorlab -- error raised explicitly by YYERROR.  |
`---------------------------------------------------*/
yyerrorlab:

  /* Pacify compilers like GCC when the user code never invokes
     YYERROR and the label yyerrorlab therefore never appears in user
     code.  */
  if (/*CONSTCOND*/ 0)
     goto yyerrorlab;

  yyerror_range[1] = yylsp[1-yylen];
  /* Do not reclaim the symbols of the rule which action triggered
     this YYERROR.  */
  YYPOPSTACK (yylen);
  yylen = 0;
  YY_STACK_PRINT (yyss, yyssp);
  yystate = *yyssp;
  goto yyerrlab1;


/*-------------------------------------------------------------.
| yyerrlab1 -- common code for both syntax error and YYERROR.  |
`-------------------------------------------------------------*/
yyerrlab1:
  yyerrstatus = 3;	/* Each real token shifted decrements this.  */

  for (;;)
    {
      yyn = yypact[yystate];
      if (!yypact_value_is_default (yyn))
	{
	  yyn += YYTERROR;
	  if (0 <= yyn && yyn <= YYLAST && yycheck[yyn] == YYTERROR)
	    {
	      yyn = yytable[yyn];
	      if (0 < yyn)
		break;
	    }
	}

      /* Pop the current state because it cannot handle the error token.  */
      if (yyssp == yyss)
	YYABORT;

      yyerror_range[1] = *yylsp;
      yydestruct ("Error: popping",
		  yystos[yystate], yyvsp, yylsp, yyscanner);
      YYPOPSTACK (1);
      yystate = *yyssp;
      YY_STACK_PRINT (yyss, yyssp);
    }

  *++yyvsp = yylval;

  yyerror_range[2] = yylloc;
  /* Using YYLLOC is tempting, but would change the location of
     the lookahead.  YYLOC is available though.  */
  YYLLOC_DEFAULT (yyloc, yyerror_range, 2);
  *++yylsp = yyloc;

  /* Shift the error token.  */
  YY_SYMBOL_PRINT ("Shifting", yystos[yyn], yyvsp, yylsp);

  yystate = yyn;
  goto yynewstate;


/*-------------------------------------.
| yyacceptlab -- YYACCEPT comes here.  |
`-------------------------------------*/
yyacceptlab:
  yyresult = 0;
  goto yyreturn;

/*-----------------------------------.
| yyabortlab -- YYABORT comes here.  |
`-----------------------------------*/
yyabortlab:
  yyresult = 1;
  goto yyreturn;

#if !defined(yyoverflow) || YYERROR_VERBOSE
/*-------------------------------------------------.
| yyexhaustedlab -- memory exhaustion comes here.  |
`-------------------------------------------------*/
yyexhaustedlab:
  yyerror (&yylloc, yyscanner, YY_("memory exhausted"));
  yyresult = 2;
  /* Fall through.  */
#endif

yyreturn:
  if (yychar != YYEMPTY)
    {
      /* Make sure we have latest lookahead translation.  See comments at
         user semantic actions for why this is necessary.  */
      yytoken = YYTRANSLATE (yychar);
      yydestruct ("Cleanup: discarding lookahead",
                  yytoken, &yylval, &yylloc, yyscanner);
    }
  /* Do not reclaim the symbols of the rule which action triggered
     this YYABORT or YYACCEPT.  */
  YYPOPSTACK (yylen);
  YY_STACK_PRINT (yyss, yyssp);
  while (yyssp != yyss)
    {
      yydestruct ("Cleanup: popping",
		  yystos[*yyssp], yyvsp, yylsp, yyscanner);
      YYPOPSTACK (1);
    }
#ifndef yyoverflow
  if (yyss != yyssa)
    YYSTACK_FREE (yyss);
#endif
#if YYERROR_VERBOSE
  if (yymsg != yymsgbuf)
    YYSTACK_FREE (yymsg);
#endif
  /* Make sure YYID is used.  */
  return YYID (yyresult);
}



/* Line 2067 of yacc.c  */
#line 261 "./mp-equation-parser.y"



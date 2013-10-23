/* A Bison parser, made by GNU Bison 2.5.  */

/* Bison interface for Yacc-like parsers in C
   
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

/* Line 2068 of yacc.c  */
#line 166 "./mp-equation-parser.y"

  MPNumber int_t;
  int integer;
  char *name;



/* Line 2068 of yacc.c  */
#line 120 "mp-equation-parser.h"
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




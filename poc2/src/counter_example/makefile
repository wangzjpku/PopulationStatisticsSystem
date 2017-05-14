AR=ar
CC=gcc
DEL=rm -rf
MK=mkdir
OUT=objs
RM=rmdir /s /q
CFLAGS=-g -Wall -O3
SRCS=$(wildcard *.c)
OBJS=$(SRCS:.c=.o)

# GNU Scientific Lib
# LINK=-lgsl -lgslcblas

LINK=
TARGET=$(notdir $(CURDIR))
LIBS=lib$(TARGET).a

all: $(TARGET)

$(TARGET): $(LIBS)
	$(CC) $(CFLAGS) counter_example.c ../counter_people.c ../channel.c ../log.c -o $@ $(LINK)

$(LIBS): $(OBJS)
	$(AR) -r $@ $^
	-$(DEL) *.o

%.o : %.c
	$(CC) $(CFLAGS) -o $@ -c $<

clean:
	-$(DEL) *.a
	-$(DEL) *.o
	-$(DEL) *.out
	-$(DEL) *.lib
	-$(DEL) $(TARGET)
	-$(DEL) $(TARGET).exe

rb: clean all

#!/bin/sh
iptables -D INPUT -i tun1 -j ACCEPT
iptables -D POSTROUTING -t nat -o tun1 -j MASQUERADE

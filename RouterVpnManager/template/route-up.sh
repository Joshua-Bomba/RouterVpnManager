#!/bin/sh
iptables -D POSTROUTING -t nat -o tun1 -j MASQUERADE
iptables -I POSTROUTING -t nat -o tun1 -j MASQUERADE
iptables -D INPUT -i tun1 -j ACCEPT
iptables -D FORWARD -i tun1 -j ACCEPT
iptables -D FORWARD -o tun1 -j ACCEPT
iptables -I INPUT -i tun1 -j ACCEPT
iptables -I FORWARD -i tun1 -j ACCEPT
iptables -I FORWARD -o tun1 -j ACCEPT

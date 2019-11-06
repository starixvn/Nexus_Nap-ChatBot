import React from 'react';

export const defaultRendererOptions = {
    blockquote: reference => reference.children,
    table: reference => reference.children,
    tableHead: reference => reference.children,
    tableBody: reference => reference.children,
    tableRow: reference => reference.children,
    tableCell: reference => reference.children,
    list: reference => reference.children,
    listItem: reference => reference.children,
    inlineCode: reference => reference.children,
    code: reference => reference.children || null,
    linkReference: reference => {
        if (!reference.href) {
            return (<>[{reference.children}]</>);
        }

        return (<a href={reference.$ref}>{reference.children}</a>);
    },
    strong: reference => {
        const length = reference.children.length;

        if (/^\s/.test(reference.children[0].props.value) || /\s$/.test(reference.children[length - 1].props.value)) {
            return (<>**{reference.children}**</>);
        }
        return (<strong>{reference.children}</strong>);
    },
    emphasis: reference => {
        const length = reference.children.length;

        if (/^\s/.test(reference.children[0].props.value) || /\s$/.test(reference.children[length - 1].props.value)) {
            return (<>*{reference.children}*</>);
        }
        return (<em>{reference.children}</em>);
    }
};
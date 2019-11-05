import React from 'react';

export const defaultRendererOptions = {
    blockquote: element => element.children,
    table: element => element.children,
    tableHead: element => element.children,
    tableBody: element => element.children,
    tableRow: element => element.children,
    tableCell: element => element.children,
    list: element => element.children,
    listItem: element => element.children,
    inlineCode: element => element.children,
    code: element => element.children,
    linkReference: element => {
        if (!element.href) {
            return (<>[{element.children}]</>);
        }

        return (<a href={element.$ref}>{element.children}</a>);
    }
};
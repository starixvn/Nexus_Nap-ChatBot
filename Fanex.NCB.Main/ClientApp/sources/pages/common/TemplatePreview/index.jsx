import React, { PureComponent } from 'react';
import remarkDisableTokenizers from 'remark-disable-tokenizers';
import ReactMarkdown from 'react-markdown/with-html';

export default class TemplatePreview extends PureComponent {
    highlightParameters = text => {
        const params = [...new Set(text.match(/{[_-\w\s]+}/g))];
        let formattedText = text;

        for (let i = 0; i < params.length; i++) {
            formattedText = formattedText.replace(new RegExp(params[i], 'g'), `<span style="color:red">${params[i]}</span>`);
        }

        formattedText = formattedText.replace(/(?:\r\n|\r|\n)/g, '<br>');

        return formattedText;
    }

    plugins = [
        [
            remarkDisableTokenizers,
            {
                block: ['blockquote', 'indentedCode', 'fencedCode', 'table', 'atxHeading', 'list', 'footnote'],
                inline: ['code', 'reference']
            }
        ]
    ]

    render() {
        return (
            <ReactMarkdown source={this.highlightParameters(this.props.text)} escapeHtml={false} linkTarget="blank"
                plugins={this.plugins}
            />
        );
    }
}
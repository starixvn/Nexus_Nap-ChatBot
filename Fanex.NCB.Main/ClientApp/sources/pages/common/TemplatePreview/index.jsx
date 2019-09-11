import React, { PureComponent } from 'react';

import ReactMarkdown from 'react-markdown/with-html';
import "react-mde/lib/styles/css/react-mde-all.css";

export default class TemplatePreview extends PureComponent {
    highlightParameters = text => {
        let params = [...new Set(text.match(/{[-\w\s]+}/g))];

        if (params) {
            for (let i = 0; i < params.length; i++) {
                text = text.replace(new RegExp(params[i], 'g'), `<span style="color:red">${params[i]}</span>`);
            }
        }

        return text;
    }

    render() {
        return (
            <ReactMarkdown source={this.highlightParameters(this.props.text)} escapeHtml={false} />
        );
    }
}
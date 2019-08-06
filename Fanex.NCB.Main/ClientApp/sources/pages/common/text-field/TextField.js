import React, { Component } from 'react';
import { Input, FormFeedback } from 'reactstrap';
import Labels from '../../common/Labels';

import './TextField.scss';

export class TextField extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="text-field">
                <Input type="textarea"
                    {...this.props}
                />
                {this.props.invalid && <FormFeedback>{Labels.FieldRequired}</FormFeedback>}
            </div>
        );
    }
}
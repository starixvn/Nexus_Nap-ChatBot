import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { SelectField } from '../../common/SelectField';
import Labels from '../../common/Labels';

export class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            notificationTypeId: {
                value: -1
            }
        };
    }

    componentWillMount() {
        this.props.getSettings();
    }

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue
        });
    }

    getParams = () => {
        return {
            notificationTypeId: this.state.notificationTypeId.value,
            pageIndex: 1,
            pageSize: 5
        };
    }

    submitHandler = e => {
        e.preventDefault();
        const params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    render() {
        return (
            <Form className="form-filter">
                <FormGroup>
                    <Label className="form-label">{Labels.NotificationType}</Label>
                    <SelectField initialValue={this.state.notificationTypeId.value}
                        data={this.props.settingData.notificationTypes}
                        isSummited={this.state.isSummited}
                        onChange={value => this.onChange('notificationTypeId', value)}
                    />
                </FormGroup>
                <button className="btn-submit" onClick={this.submitHandler}>{Labels.Submit}</button>
                <button className="btn-new">{Labels.CreateNewTemplate}</button>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { settingData } = state;
    return {
        settingData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);
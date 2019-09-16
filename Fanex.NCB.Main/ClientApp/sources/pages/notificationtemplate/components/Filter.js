import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Input } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import Labels from '../../common/Labels';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            notificationTypeId: ''
        };
    }

    componentDidMount() {
        const params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    handleNotificationTypeChange = event =>
        this.setState({
            notificationTypeId: event.target.value
        });

    getParams = () => {
        return {
            notificationTypeId: parseInt(this.state.notificationTypeId, 10),
            pageIndex: 1,
            pageSize: 10
        };
    }

    submitHandler = e => {
        e.preventDefault();
        const params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    renderNotificationTypeOption = item => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    render() {
        return (
            <Form className="form-filter">
                <FormGroup>
                    <Label className="form-label">{Labels.NotificationType}</Label>
                    <Input type="select"
                        name="notificationTypeId"
                        onChange={this.handleNotificationTypeChange}
                        value={this.state.notificationTypeId}
                    >
                        <option value="">{Labels.All}</option>
                        {this.props.notificationTypes.map(this.renderNotificationTypeOption)}

                    </Input>
                </FormGroup>
                <button className="btn-submit" onClick={this.submitHandler}>{Labels.Submit}</button>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { notificationTypes, gridParams } = state;
    return {
        notificationTypes,
        gridParams
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);
import React, { Component } from 'react';
import './AppContainer.scss';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import DataGrid from './DataGrid';
import Filter from './Filter';
import EditNewMessage from './EditNewMessage';

export class AppContainer extends Component {

    constructor(props) {
        super(props);

        this.haveHostPermission = document.getElementById('have-host-permission') && document.getElementById('have-host-permission').checked;
    }

    render() {
        const { editNewMessagePopup } = this.props;
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Reset Last ID" />
                <Filter/>
                <DataGrid haveHostPermission={this.haveHostPermission}/>
                {
                    editNewMessagePopup.show &&
                    <EditNewMessage
                        id={editNewMessagePopup.jobIdentify}
                        title={editNewMessagePopup.title}
                        newMaxId={editNewMessagePopup.newMaxMessageId}
                        data={editNewMessagePopup.data}
                        settingName={editNewMessagePopup.settingName}
                    />
                }
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { editNewMessagePopup } = state;
    return { editNewMessagePopup };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);
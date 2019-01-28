import React, { Component } from 'react';
import NewNotification from './components/NewNotification';
import { connect } from 'react-redux';

class App extends Component {
  constructor(props) {
    super(props);      
    this.state = {
      isShow: false,
      titlePopup: '',
      contentPopup: '',
      editConfirmPopup: {
        title: "Edit Title & Content",
        content: "`You can <strong>edit Title & Content</strong> but manually change the text in {} might cause the parameter replacing feature dysfunctional`"
      },
      resetConfirmPopup: {
        title: "Reset Title & Content",
        content: "Reset <strong>Title & Content</strong> will recover the original template and discard your change"
      }
    };

    this.toggle =  this.toggle.bind(this);
    this.contentEditConfirmPopup = this.contentEditConfirmPopup.bind(this);
    this.contentResetConfirmPopup = this.contentResetConfirmPopup.bind(this);
  }

  toggle() {
    this.setState({
      isShow: !this.state.isShow
    });
  }

  contentEditConfirmPopup() {
    this.toggle();

    this.setState({
      titlePopup: this.state.editConfirmPopup.title,
      contentPopup: this.state.editConfirmPopup.content
    });
  }

  contentResetConfirmPopup() {
    this.toggle();

    this.setState({
      titlePopup: this.state.resetConfirmPopup.title,
      contentPopup: this.state.resetConfirmPopup.content
    });
  }

  render() { 
    return (      
       <NewNotification
          isShowPopup={this.state.isShow}
          titlePopup={this.state.titlePopup}
          contentPopup={this.state.contentPopup}
          toggle={this.toggle}
          contentEditConfirmPopupHandler={this.contentEditConfirmPopup}
          contentResetConfirmPopupHandler={this.contentResetConfirmPopup}
          />
    );
  }
}

export default connect(
  state => state.login
)(App);
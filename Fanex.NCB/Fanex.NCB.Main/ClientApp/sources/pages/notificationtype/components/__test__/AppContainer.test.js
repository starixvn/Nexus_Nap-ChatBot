import React from 'react';
import {AppContainer} from '../AppContainer';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe('AppContainer', () => {   
    describe('render', () => {
        it('hide UpdatePopup, DeletePopup', () => {
            const props = {                
                setGridParams: jest.fn(),
                getGridData: jest.fn(),
                updatePopupData: {isShowed: false},
                deletePopupData: {isShowed: false}
            };            
            const appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />);            

            expect(appContainer).toMatchSnapshot();
        })

        it('show UpdatePopup', () => {
            const props = {                
                setGridParams: jest.fn(),
                getGridData: jest.fn(),
                updatePopupData: {isShowed: true},
                deletePopupData: {isShowed: false}
            };
            const appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />);            
            
            expect(appContainer).toMatchSnapshot();
        })

        it('show DeletePopup', () => {
            const props = {                
                setGridParams: jest.fn(),
                getGridData: jest.fn(),
                updatePopupData: {isShowed: false},
                deletePopupData: {isShowed: true}
            };
            const appContainer = (new ShallowRenderer()).render(<AppContainer {...props} />);            
            
            expect(appContainer).toMatchSnapshot();
        })
      
    })
})